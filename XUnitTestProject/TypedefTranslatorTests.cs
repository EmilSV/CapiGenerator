using CapiGenerator;
using CapiGenerator.CModel;
using CapiGenerator.CSModel;
using CapiGenerator.Parser;
using CapiGenerator.Translator;
using CppAst;

namespace XUnitTestProject;

public sealed class TypedefTranslatorTests
{
    [Fact]
    public void PrimitiveTypedefConvertsImplicitlyBothWays()
    {
        var csStruct = TranslateTypedefStructs("""
            typedef unsigned int Handle;
            """).Single(item => item.Name == "Handle");

        Assert.Equal(
            ["Handle(uint)", "uint(Handle)"],
            GetConversionSignatures(csStruct));
    }

    [Fact]
    public void NestedTypedefConvertsToEveryTypeInTheChain()
    {
        var csStruct = TranslateTypedefStructs("""
            typedef unsigned long long Uint64;
            typedef Uint64 WindowFlags;
            """).Single(item => item.Name == "WindowFlags");

        Assert.Equal(
            ["Uint64(WindowFlags)", "WindowFlags(Uint64)", "WindowFlags(ulong)", "ulong(WindowFlags)"],
            GetConversionSignatures(csStruct));
    }

    [Fact]
    public void FunctionPointerTypedefConvertsImplicitly()
    {
        var csStruct = Assert.Single(TranslateTypedefStructs("""
            typedef int (*Callback)(void *userdata);
            """));

        Assert.Equal(2, csStruct.Methods.Count);
        Assert.All(csStruct.Methods, method => Assert.Equal(CSMethodOperatorModifier.Implicit, method.OperatorModifier));
        Assert.True(csStruct.IsUnsafe);
    }

    [Fact]
    public void NestedPointerTypedefMarksWrapperUnsafe()
    {
        var csStruct = TranslateTypedefStructs("""
            typedef void *Context;
            typedef Context SharedContext;
            """).Single(item => item.Name == "SharedContext");

        Assert.Equal(4, csStruct.Methods.Count);
        Assert.True(csStruct.IsUnsafe);
    }

    private static string[] GetConversionSignatures(CSStruct csStruct)
    {
        return csStruct.Methods
            .Select(method =>
            {
                Assert.Equal(CSMethodOperatorModifier.Implicit, method.OperatorModifier);
                var parameter = Assert.Single(method.Parameters.ToArray());
                return $"{GetShortName(method.ReturnType)}({GetShortName(parameter.Type)})";
            })
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

    private static string GetShortName(CSTypeInstance type)
    {
        var name = type.ToString()!;
        return name[(name.LastIndexOf('.') + 1)..];
    }

    private static CSStruct[] TranslateTypedefStructs(string header)
    {
        var compilationUnit = ParseHeader(header);
        var translationUnit = new CSTranslationUnit();
        translationUnit.AddTranslator(new CSTypedefTranslator());
        translationUnit.Translate([compilationUnit]);
        return translationUnit.GetCSStructsEnumerable().ToArray();
    }

    private static CCompilationUnit ParseHeader(string header)
    {
        var headerPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.h");
        File.WriteAllText(headerPath, header);

        try
        {
            var fakeCStdHeaderPath = FakeCStdHeader.CreateFakeStdHeaderFolder();
            var options = new CppParserOptions();
            options.IncludeFolders.Add(fakeCStdHeaderPath);

            var cppCompilation = CppParser.ParseFile(headerPath, options);
            Assert.False(
                cppCompilation.HasErrors,
                string.Join(Environment.NewLine, cppCompilation.Diagnostics.Messages.Select(message => message.ToString())));

            var compilationUnit = new CCompilationUnit();
            compilationUnit.AddParser([new TypedefParser()]);
            compilationUnit.Parse([cppCompilation]);

            return compilationUnit;
        }
        finally
        {
            File.Delete(headerPath);
        }
    }
}
