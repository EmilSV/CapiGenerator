using CapiGenerator;
using CapiGenerator.Parser;
using CapiGenerator.Translator;
using CppAst;

namespace XUnitTestProject;

public sealed class ConstantParserTests
{
    [Fact]
    public void ReferencedDefineCreatesTranslatedSdlMaxTimeConstant()
    {
        var options = new CppParserOptions
        {
            ParseMacros = true,
        };
        options.IncludeFolders.Add(FakeCStdHeader.CreateFakeStdHeaderFolder());

        var headerPath = Path.Combine(AppContext.BaseDirectory, "TestHeader", "DefineRefConst.h");
        var cppCompilation = CppParser.ParseFile(headerPath, options);
        Assert.False(
            cppCompilation.HasErrors,
            string.Join(Environment.NewLine, cppCompilation.Diagnostics.Messages.Select(message => message.ToString())));

        var compilationUnit = new CCompilationUnit();
        compilationUnit.AddParser(new ConstantParser());
        compilationUnit.Parse([cppCompilation]);

        var translationUnit = new CSTranslationUnit();
        translationUnit.AddTranslator(new CSConstTranslator("Constants"));
        translationUnit.Translate([compilationUnit]);

        var constantsClass = Assert.Single(translationUnit.GetCSStaticClassesEnumerable());
        Assert.Contains(constantsClass.Fields, field => field.Name == "SDL_MAX_TIME");
    }
}
