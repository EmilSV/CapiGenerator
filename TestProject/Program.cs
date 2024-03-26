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

var options = new CppParserOptions
{
    ParseMacros = true,
};

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
    new TypedefParser()
]);


compilationUnit.Parse([cppCompilation]);

foreach (var constant in compilationUnit.GetEnumEnumerable())
{
    Console.WriteLine(constant.Name);
}

var translationUnit = new CSTranslationUnit();

translationUnit.AddTranslator([
    new CSConstTranslator("TestProject"),
    new CSEnumTranslator(),
    new CSFunctionTranslator("TestProject", "TestProject.Interop"),
    new CSStructTranslator(),
    new CSTypedefTranslator()
]);

translationUnit.Translate([compilationUnit]);

var writer = new CSEnumWriter();

foreach (var csEnum in translationUnit.GetCSEnumEnumerable())
{

    void updateNames()
    {
        foreach (var enumValue in csEnum.Values)
        {
            if (enumValue.Name.Value.StartsWith($"{csEnum.Name}_"))
            {
                enumValue.Name.SetValue(enumValue.Name.Value.Replace($"{csEnum.Name}_", ""));
            }
        }
    }

    updateNames();


    await writer.Write(csEnum, new CSWriteConfig
    {
        OutputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "output"),
        Usings = [
            "System"
        ]
    });
}

return 0;