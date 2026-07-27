using CapiGenerator;
using CapiGenerator.CModel;
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
        Assert.Equal("constants.h", token.SourceLocation.File);
        Assert.Equal(10, token.SourceLocation.Line);
        Assert.Equal(5, token.SourceLocation.Column);
    }

    [Fact]
    public void TypedefCastCppTokensBecomeSingleUnresolvedCastToken()
    {
        var sourceLocation = new CppSourceLocation("constants.h", 0, 1, 1);
        CppToken[] cppTokens =
        [
            new(CppTokenKind.Punctuation, "("),
            new(CppTokenKind.Identifier, "TestSint8"),
            new(CppTokenKind.Punctuation, ")"),
        ];

        var converted = BaseCConstantToken.TryConvert(
            cppTokens,
            new HashSet<string> { "TestSint8" },
            sourceLocation,
            out var constantTokens);

        Assert.True(converted);
        var castToken = Assert.IsType<CConstCastToken>(Assert.Single(constantTokens));
        Assert.False(castToken.TryGetConstantType(out _));
    }

    [Fact]
    public void MultiTokenPrimitiveCastCppTokensBecomeSingleResolvedCastToken()
    {
        var sourceLocation = new CppSourceLocation("constants.h", 0, 1, 1);
        CppToken[] cppTokens =
        [
            new(CppTokenKind.Punctuation, "("),
            new(CppTokenKind.Identifier, "unsigned"),
            new(CppTokenKind.Identifier, "long"),
            new(CppTokenKind.Identifier, "long"),
            new(CppTokenKind.Punctuation, ")"),
        ];

        var converted = BaseCConstantToken.TryConvert(
            cppTokens,
            new HashSet<string>(),
            sourceLocation,
            out var constantTokens);

        Assert.True(converted);
        var castToken = Assert.IsType<CConstCastToken>(Assert.Single(constantTokens));
        Assert.True(castToken.TryGetConstantType(out var constantType));
        Assert.Equal(CConstantType.UnsignedLongLong, constantType);
    }

    [Fact]
    public void ParenthesizedConstantRemainsIdentifierAndPunctuationTokens()
    {
        var sourceLocation = new CppSourceLocation("constants.h", 0, 1, 1);
        CppToken[] cppTokens =
        [
            new(CppTokenKind.Punctuation, "("),
            new(CppTokenKind.Identifier, "TEST_VALUE"),
            new(CppTokenKind.Punctuation, ")"),
        ];

        var converted = BaseCConstantToken.TryConvert(
            cppTokens,
            new HashSet<string>(),
            sourceLocation,
            out var constantTokens);

        Assert.True(converted);
        Assert.Collection(
            constantTokens,
            token => Assert.IsType<CConstantPunctuationToken>(token),
            token => Assert.IsType<CConstIdentifierToken>(token),
            token => Assert.IsType<CConstantPunctuationToken>(token));
    }

    [Fact]
    public void ReferencedDefineCreatesTranslatedTestMaxTimeConstant()
    {
        var constants = TranslateConstants();

        Assert.Contains("TEST_MAX_TIME", constants.Keys);
    }

    [Fact]
    public void UInt64MacroHandlesMaximumValue()
    {
        var constants = TranslateConstants();

        Assert.Equal("0xFFFFFFFFFFFFFFFF", constants["TEST_MAX_UINT64"]);
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
        Assert.DoesNotContain("TEST_FORMAT_ANNOTATION", constants.Keys);
        Assert.DoesNotContain("TEST_EMPTY_ANNOTATION", constants.Keys);
        Assert.DoesNotContain("TEST_CALL_CONVENTION", constants.Keys);
        Assert.DoesNotContain("TEST_ATTRIBUTE", constants.Keys);
        Assert.DoesNotContain("TEST_PLATFORM_PREDICATE", constants.Keys);
        Assert.DoesNotContain("TEST_WRAPPED_ANNOTATION", constants.Keys);
    }

    [Fact]
    public void TypedefCastIsTranslatedAsPrimitiveCast()
    {
        var constants = TranslateConstants();

        Assert.Equal("( (sbyte) 0x7F )", constants["TEST_MAX_SINT8"]);
    }

    [Fact]
    public void ChainedTypedefCastResolvesToPrimitiveCast()
    {
        var constants = TranslateConstants();

        Assert.Equal("( (sbyte) 0x7F )", constants["TEST_MAX_SINT8_ALIAS"]);
    }

    [Fact]
    public void ObjectMacroNameTakesPrecedenceOverTypedefCastDetection()
    {
        var constants = TranslateConstants();

        Assert.Equal("( Constants.TestSharedName )", constants["TEST_GROUPED_SHARED_NAME"]);
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
        compilationUnit.AddParser([
            new ConstantParser(),
            new TypedefParser(),
        ]);
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
