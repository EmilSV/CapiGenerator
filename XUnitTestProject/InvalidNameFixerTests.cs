using CapiGenerator;
using CapiGenerator.Fixer;
using CapiGenerator.Parser;
using CapiGenerator.Translator;
using CppAst;

namespace XUnitTestProject;

public sealed class InvalidNameFixerTests
{
    [Fact]
    public void KeywordNamesBecomeVerbatimAndContextualKeywordsAreKept()
    {
        var translationUnit = Translate("""
            typedef struct Names { int value; int event; int file; } Names;
            extern void function(int base, int value, int string);
            """);

        var structs = translationUnit.GetCSStructsEnumerable().ToList();
        var staticClasses = translationUnit.GetCSStaticClassesEnumerable().ToList();
        InvalidNameFixer.FixInvalidNames(structs);
        InvalidNameFixer.FixInvalidNames(staticClasses);

        var names = Assert.Single(structs, item => item.Name == "Names");
        Assert.Equal(["value", "@event", "file"], names.Fields.Select(field => field.Name));

        var function = Assert.Single(Assert.Single(staticClasses).Methods);
        Assert.Equal(["@base", "value", "@string"], function.Parameters.Select(parameter => parameter.Name));
    }

    [Fact]
    public void CustomInvalidNamesStillGetSuffix()
    {
        var translationUnit = Translate("typedef struct Names { int custom; } Names;");

        var structs = translationUnit.GetCSStructsEnumerable().ToList();
        InvalidNameFixer.FixInvalidNames(structs, invalidNames: ["custom"]);

        var names = Assert.Single(structs, item => item.Name == "Names");
        Assert.Equal("customValue", Assert.Single(names.Fields).Name);
    }

    private static CSTranslationUnit Translate(string header)
    {
        var headerPath = Path.Combine(Path.GetTempPath(), $"names-{Guid.NewGuid():N}.h");

        try
        {
            File.WriteAllText(headerPath, header);

            var options = new CppParserOptions();
            options.IncludeFolders.Add(FakeCStdHeader.CreateFakeStdHeaderFolder());

            var cppCompilation = CppParser.ParseFile(headerPath, options);
            Assert.False(
                cppCompilation.HasErrors,
                string.Join(Environment.NewLine, cppCompilation.Diagnostics.Messages.Select(message => message.ToString())));

            var compilationUnit = new CCompilationUnit();
            compilationUnit.AddParser([new StructParser(), new FunctionParser(), new TypedefParser()]);
            compilationUnit.Parse([cppCompilation]);

            var translationUnit = new CSTranslationUnit();
            translationUnit.AddTranslator([
                new CSConstAndFunctionTranslator("Native", "native"),
                new CSStructTranslator(),
                new CSTypedefTranslator(),
            ]);
            translationUnit.Translate([compilationUnit]);
            return translationUnit;
        }
        finally
        {
            File.Delete(headerPath);
        }
    }
}
