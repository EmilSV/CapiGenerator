using CapiGenerator;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Parser;
using CapiGenerator.Translator;
using CppAst;

namespace XUnitTestProject;

public sealed class ConstantParserTests
{
    [Theory]
    [InlineData(CppTokenKind.Identifier, "VALUE")]
    [InlineData(CppTokenKind.Literal, "42")]
    [InlineData(CppTokenKind.Punctuation, "+")]
    public void ConstantTokensRetainDebugInfo(CppTokenKind kind, string text)
    {
        var debugInfo = new CppSourceLocation("constants.h", 100, 10, 5);

        var token = BaseCConstantToken.From(new CppToken(kind, text), debugInfo);

        Assert.NotNull(token);
        Assert.Equal("constants.h", token.DebugInfo.File);
        Assert.Equal(10, token.DebugInfo.Line);
        Assert.Equal(5, token.DebugInfo.Column);
    }

    [Fact]
    public void ReferencedDefineCreatesTranslatedTestMaxTimeConstant()
    {
        var constants = TranslateConstants();

        Assert.Contains("TEST_MAX_TIME", constants.Keys);
    }

    [Fact]
    public void MultiParameterMacroExpandsArguments()
    {
        var constants = TranslateConstants();

        Assert.Equal("( 1 + 2 )", constants["TEST_ADD_VALUE"]);
    }

    [Fact]
    public void NestedMacroFunctionsExpandRecursively()
    {
        var constants = TranslateConstants();

        Assert.Equal("( 1 + 2 )", constants["TEST_NESTED_ADD"]);
    }

    [Fact]
    public void ZeroParameterMacroExpands()
    {
        var constants = TranslateConstants();

        Assert.Equal("7", constants["TEST_ZERO_VALUE"]);
    }

    [Fact]
    public void InvalidMacroInvocationsAreNotTranslated()
    {
        var constants = TranslateConstants();

        Assert.DoesNotContain("TEST_RECURSIVE_VALUE", constants.Keys);
        Assert.DoesNotContain("TEST_WRONG_ARITY", constants.Keys);
    }

    private static IReadOnlyDictionary<string, string> TranslateConstants()
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
        return constantsClass.Fields.ToDictionary(
            field => field.Name,
            field => field.DefaultValue.Value?.ToString() ?? string.Empty);
    }
}
