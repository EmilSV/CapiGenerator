using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CapiGenerator;
using CapiGenerator.CModel.Comments;
using CapiGenerator.Parser;
using CapiGenerator.Translator;
using CapiGenerator.Writer;
using CppAst;


string headerFile = Path.Combine(Directory.GetCurrentDirectory(), args[0]);

if (!File.Exists(headerFile))
{
    Console.WriteLine($"Header file {headerFile} does not exist.");
    return 1;
}

string headerPath = FakeCStdHeader.CreateFakeStdHeaderFolder();

var options = new CppParserOptions
{
    ParseMacros = true,
    ParseComments = true,
};

options.IncludeFolders.Add(headerPath);

var cppCompilation = CppParser.ParseFile(headerFile, options);

if (cppCompilation.HasErrors)
{
    Console.WriteLine("Errors occurred while parsing the header file.");

    foreach (var error in cppCompilation.Diagnostics.Messages)
    {
        Console.WriteLine(error);
    }

    return 1;
}

var compilationUnit = new CCompilationUnit();

compilationUnit.AddParser([
    new ConstantParser(),
    new EnumParser(),
    new FunctionParser(),
    new StructParser(),
    new UnionParser(),
    new TypedefParser()
]);


compilationUnit.Parse([cppCompilation]);

var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output");
Directory.CreateDirectory(outputDirectory);

var functionComments = compilationUnit.GetFunctionEnumerable()
    .Select(function => new
    {
        function.Name,
        Comment = ToJsonComment(function.Comment)
    });

await File.WriteAllTextAsync(
    Path.Combine(outputDirectory, "function-comments.json"),
    JsonSerializer.Serialize(functionComments, new JsonSerializerOptions
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    }));

foreach (var constant in compilationUnit.GetEnumEnumerable())
{
    Console.WriteLine(constant.Name);
}

var translationUnit = new CSTranslationUnit();

translationUnit.AddTranslator([
    new CSConstTranslator("TestProjectConstants"),
    new CSEnumTranslator(),
    new CSFunctionTranslator("TestProjectFunction", "TestProject.Interop"),
    new CSStructTranslator(),
    new CSUnionTranslator(),
    new CSTypedefTranslator()
]);

translationUnit.Translate([compilationUnit]);

var structWriter = new CSStructWriter();
var enumWriter = new CSEnumWriter();

foreach (var csStruct in translationUnit.GetCSStructsEnumerable())
{
    csStruct.Namespace = "TestProject";

    await structWriter.Write(csStruct, new CSWriteConfig
    {
        OutputDirectory = outputDirectory,
        Usings = [
            "System"
        ]
    });
}

foreach (var csEnum in translationUnit.GetCSEnumsEnumerable())
{
    csEnum.Namespace = "TestProject";

    await enumWriter.Write(csEnum, new CSWriteConfig
    {
        OutputDirectory = outputDirectory,
        Usings = [
            "System"
        ]
    });
}

foreach (var csStaticClass in translationUnit.GetCSStaticClassesEnumerable())
{
    csStaticClass.Namespace = "TestProject";

    var staticClassWriter = new CSStaticClassWriter();

    await staticClassWriter.Write(csStaticClass, new CSWriteConfig
    {
        OutputDirectory = outputDirectory,
        Usings = [
            "System"
        ]
    });
}

return 0;

static object? ToJsonComment(CBaseComment? comment) => comment switch
{
    null => null,
    CBlockCommandComment blockCommand => new
    {
        Kind = nameof(CBlockCommandComment),
        blockCommand.CommandName,
        blockCommand.Arguments,
        Children = ToJsonChildren(blockCommand)
    },
    CFullComment full => new
    {
        Kind = nameof(CFullComment),
        Children = ToJsonChildren(full)
    },
    CHtmlEndTagComment htmlEndTag => new
    {
        Kind = nameof(CHtmlEndTagComment),
        htmlEndTag.TagName,
        Children = ToJsonChildren(htmlEndTag)
    },
    CHtmlStartTagComment htmlStartTag => new
    {
        Kind = nameof(CHtmlStartTagComment),
        htmlStartTag.TagName,
        htmlStartTag.IsSelfClosing,
        htmlStartTag.Attributes,
        Children = ToJsonChildren(htmlStartTag)
    },
    CInlineCommandComment inlineCommand => new
    {
        Kind = nameof(CInlineCommandComment),
        inlineCommand.RenderKind,
        inlineCommand.CommandName,
        inlineCommand.Arguments,
        Children = ToJsonChildren(inlineCommand)
    },
    CParagraphComment paragraph => new
    {
        Kind = nameof(CParagraphComment),
        Children = ToJsonChildren(paragraph)
    },
    CParamCommandComment paramCommand => new
    {
        Kind = nameof(CParamCommandComment),
        paramCommand.ParamName,
        paramCommand.IsParamIndexValid,
        paramCommand.ParamIndex,
        paramCommand.Direction,
        paramCommand.IsDirectionExplicit,
        Children = ToJsonChildren(paramCommand)
    },
    CTemplateParamCommandComment templateParamCommand => new
    {
        Kind = nameof(CTemplateParamCommandComment),
        templateParamCommand.ParamName,
        templateParamCommand.Depth,
        templateParamCommand.IsPositionValid,
        templateParamCommand.Index,
        templateParamCommand.CommandName,
        templateParamCommand.Arguments,
        Children = ToJsonChildren(templateParamCommand)
    },
    CTextComment text => new
    {
        Kind = nameof(CTextComment),
        text.Text,
        Children = ToJsonChildren(text)
    },
    CVerbatimBlockCommandComment verbatimBlockCommand => new
    {
        Kind = nameof(CVerbatimBlockCommandComment),
        verbatimBlockCommand.CommandName,
        verbatimBlockCommand.Arguments,
        Children = ToJsonChildren(verbatimBlockCommand)
    },
    CVerbatimBlockLineComment verbatimBlockLine => new
    {
        Kind = nameof(CVerbatimBlockLineComment),
        verbatimBlockLine.Text,
        Children = ToJsonChildren(verbatimBlockLine)
    },
    CVerbatimLineComment verbatimLine => new
    {
        Kind = nameof(CVerbatimLineComment),
        verbatimLine.Text,
        Children = ToJsonChildren(verbatimLine)
    },
    _ => throw new NotSupportedException($"Unsupported comment type: {comment.GetType().FullName}")
};

static object[] ToJsonChildren(CBaseComment comment) =>
    comment.Children.Select(child => ToJsonComment(child)!).ToArray();
