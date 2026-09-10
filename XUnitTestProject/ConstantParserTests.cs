using CapiGenerator;
using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.ConstantToken;
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
    [InlineData(CppTokenKind.Punctuation, ",")]
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
    public void FloatLiteralRetainsCSharpFloatSuffix()
    {
        var token = new CConstLiteralToken(
            "9.80665f",
            new CppSourceLocation("constants.h", 0, 1, 1));

        Assert.Equal(CConstantType.Float, token.Type);
        Assert.Equal("9.80665f", token.Value);
    }

    [Fact]
    public void DoubleLiteralDoesNotGainFloatSuffix()
    {
        var token = new CConstLiteralToken(
            "3.141592653589793",
            new CppSourceLocation("constants.h", 0, 1, 1));

        Assert.Equal(CConstantType.Double, token.Type);
        Assert.Equal("3.141592653589793", token.Value);
    }

    [Theory]
    [InlineData("1u", CConstantType.UnsignedInt, "1u")]
    [InlineData("1ULL", CConstantType.UnsignedLongLong, "1ul")]
    public void UnsignedLiteralRetainsCSharpSuffix(
        string value,
        CConstantType expectedType,
        string expectedValue)
    {
        var token = new CConstLiteralToken(
            value,
            new CppSourceLocation("constants.h", 0, 1, 1));

        Assert.Equal(expectedType, token.Type);
        Assert.Equal(expectedValue, token.Value);
        Assert.Equal(expectedValue, CSConstLiteralToken.FromCConstantLiteralToken(token).ToString());
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
    public void SizeTCastCppTokensBecomeResolvedCastToken()
    {
        var sourceLocation = new CppSourceLocation("constants.h", 0, 1, 1);
        CppToken[] cppTokens =
        [
            new(CppTokenKind.Punctuation, "("),
            new(CppTokenKind.Identifier, "size_t"),
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
        Assert.Equal(CConstantType.Size_t, constantType);
    }

    [Theory]
    [InlineData(CConstantType.UInt32_t, "1", "unchecked ( (uint) - 1 )")]
    [InlineData(CConstantType.UInt32_t, "2", "unchecked ( (uint) - 2 )")]
    [InlineData(CConstantType.UInt64_t, "1", "unchecked ( (ulong) - 1 )")]
    [InlineData(CConstantType.UInt64_t, "2", "unchecked ( (ulong) - 2 )")]
    [InlineData(CConstantType.Size_t, "1", "unchecked ( (nuint) ( - 1 ) )")]
    public void UnsignedCastOfNegativeIntegerUsesEquivalentUnsignedLiteral(
        CConstantType castType,
        string magnitude,
        string expected)
    {
        var expression = new CSConstantExpression([
            new CSConstCastToken(castType),
            new CSConstantPunctuationToken { Type = CSPunctuationType.Minus },
            new CSConstLiteralToken(magnitude, CSConstantType.Int),
        ]);

        Assert.Equal(expected, expression.ToString());
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
    public void BuiltinMacroResolverStillEvaluatesSingleArgumentMacros()
    {
        var sourceLocation = new CppSourceLocation("constants.h", 0, 1, 1);
        BaseCConstantToken[] tokens =
        [
            new CConstIdentifierToken("UINT64_C", sourceLocation),
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.LeftParenthesis },
            new CConstLiteralToken("42", sourceLocation),
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis },
        ];

        var result = MacroFunctionResolver.ResolveMacroFunction(tokens);

        var literal = Assert.IsType<CConstLiteralToken>(Assert.Single(result));
        Assert.Equal("42", literal.Value);
        Assert.Equal(CConstantType.UInt64_t, literal.Type);
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

    [Fact]
    public void MacroAliasingFakeFunctionIsNotParsedAsConstantOrFunction()
    {
        var fakeHeaderFolder = FakeCStdHeader.CreateFakeStdHeaderFolder();
        var headerPath = Path.Combine(Path.GetTempPath(), $"fake-function-alias-{Guid.NewGuid():N}.h");

        try
        {
            File.WriteAllText(headerPath, "#include <malloc.h>\n#define alloca _alloca\n");

            var options = new CppParserOptions
            {
                ParseMacros = true,
            };
            options.IncludeFolders.Add(fakeHeaderFolder);

            var cppCompilation = CppParser.ParseFile(headerPath, options);
            Assert.False(
                cppCompilation.HasErrors,
                string.Join(Environment.NewLine, cppCompilation.Diagnostics.Messages.Select(message => message.ToString())));
            Assert.Contains(cppCompilation.Functions, function => function.Name == "_alloca");

            var compilationUnit = new CCompilationUnit();
            compilationUnit.AddParser([
                new ConstantParser(),
                new FunctionParser(),
            ]);
            compilationUnit.Parse([cppCompilation]);

            Assert.Null(compilationUnit.GetConstantByName("alloca"));
            Assert.Null(compilationUnit.GetFunctionByName("_alloca"));
        }
        finally
        {
            File.Delete(headerPath);
        }
    }

    [Fact]
    public void MacroAliasingEmptyFakeMacroIsNotParsedAsConstant()
    {
        var fakeHeaderFolder = FakeCStdHeader.CreateFakeStdHeaderFolder();
        var headerPath = Path.Combine(Path.GetTempPath(), $"fake-empty-alias-{Guid.NewGuid():N}.h");

        try
        {
            File.WriteAllText(
                headerPath,
                "#include <sal.h>\n#define FORMAT_STRING _Printf_format_string_\n");

            var options = new CppParserOptions
            {
                ParseMacros = true,
            };
            options.IncludeFolders.Add(fakeHeaderFolder);

            var cppCompilation = CppParser.ParseFile(headerPath, options);
            Assert.False(
                cppCompilation.HasErrors,
                string.Join(Environment.NewLine, cppCompilation.Diagnostics.Messages.Select(message => message.ToString())));

            var compilationUnit = new CCompilationUnit();
            compilationUnit.AddParser(new ConstantParser());
            compilationUnit.Parse([cppCompilation]);

            Assert.Null(compilationUnit.GetConstantByName("FORMAT_STRING"));
        }
        finally
        {
            File.Delete(headerPath);
        }
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
