using CapiGenerator;
using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinMacroFunctions;
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

    [Theory]
    [InlineData("INT64_C", "42", CConstantType.Int64_t)]
    [InlineData("INT64_C", "9223372036854775807", CConstantType.Int64_t)]
    [InlineData("INT64_C", "0x7FFFFFFFFFFFFFFF", CConstantType.Int64_t)]
    [InlineData("UINT64_C", "42", CConstantType.UInt64_t)]
    [InlineData("UINT64_C", "18446744073709551615", CConstantType.UInt64_t)]
    [InlineData("UINT64_C", "0xFFFFFFFFFFFFFFFF", CConstantType.UInt64_t)]
    [InlineData("UINT32_C", "42", CConstantType.UInt32_t)]
    [InlineData("UINT32_C", "4294967295", CConstantType.UInt32_t)]
    public void IntegerMacroProducesTypedLiteralForSingleNumber(string name, string value, CConstantType expectedType)
    {
        var sourceLocation = new CppSourceLocation("constants.h", 0, 1, 1);
        var literal = new CConstLiteralToken(value, sourceLocation);
        BaseCConstantToken[] tokens =
        [
            new CConstIdentifierToken(name, sourceLocation),
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.LeftParenthesis },
            literal,
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis },
        ];

        var result = MacroFunctionResolver.ResolveMacroFunction(tokens);
        var expression = new CConstantExpression(result);

        var resolvedLiteral = Assert.IsType<CConstLiteralToken>(Assert.Single(result));
        Assert.Equal(literal.Value, resolvedLiteral.Value);
        Assert.Equal(expectedType, resolvedLiteral.Type);
        Assert.Equal(sourceLocation, resolvedLiteral.SourceLocation);
        Assert.True(expression.IsResolved());
        Assert.Equal(expectedType, expression.GetTypeOfExpression());
        Assert.Equal(value, CSConstantExpression.FromCConstantExpression(expression).ToString());
    }

    [Theory]
    [InlineData("1.5f", CConstantType.Float)]
    [InlineData("1.5", CConstantType.Double)]
    [InlineData("\"text\"", CConstantType.String)]
    [InlineData("'a'", CConstantType.Char)]
    [InlineData("invalid", CConstantType.Unknown)]
    public void IntegerMacrosRejectNonIntegerLiterals(string value, CConstantType type)
    {
        var sourceLocation = new CppSourceLocation("constants.h", 0, 1, 1);
        var literal = new CConstLiteralToken(value, type, sourceLocation);
        BaseCConstantToken[] expression =
        [
            literal,
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.Plus },
            new CConstLiteralToken("1", sourceLocation),
        ];

        foreach (var handler in AllBuiltinMacroFunctions.Functions)
        {
            Assert.False(handler.TryEvaluate([[literal]], out var result));
            Assert.Null(result);
            Assert.False(handler.TryEvaluate([expression], out result));
            Assert.Null(result);
        }
    }

    [Theory]
    [InlineData("INT64_C")]
    [InlineData("UINT64_C")]
    [InlineData("UINT32_C")]
    public void IntegerMacroRequiresOneNonEmptyArgument(string name)
    {
        var sourceLocation = new CppSourceLocation("constants.h", 0, 1, 1);
        var literal = new CConstLiteralToken("1", sourceLocation);
        var handler = AllBuiltinMacroFunctions.Functions.Single(handler => handler.Name == name);
        IReadOnlyList<IReadOnlyList<BaseCConstantToken>>[] invalidArguments =
        [
            [],
            [[]],
            [[literal], [literal]],
        ];

        foreach (var arguments in invalidArguments)
        {
            Assert.False(handler.TryEvaluate(arguments, out var result));
            Assert.Null(result);
        }
    }

    [Theory]
    [InlineData("INT64_C", "long", CConstantType.LongLong)]
    [InlineData("UINT64_C", "ulong", CConstantType.UnsignedLongLong)]
    [InlineData("UINT32_C", "uint", CConstantType.UnsignedInt)]
    public void IntegerMacroCastsWholeArithmeticExpression(string name, string castType, CConstantType expectedType)
    {
        var sourceLocation = new CppSourceLocation("constants.h", 0, 1, 1);
        BaseCConstantToken[] tokens =
        [
            new CConstIdentifierToken(name, sourceLocation),
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.LeftParenthesis },
            new CConstLiteralToken("1", sourceLocation),
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.Plus },
            new CConstLiteralToken("2", sourceLocation),
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis },
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.Multiply },
            new CConstLiteralToken("3", sourceLocation),
        ];

        var result = MacroFunctionResolver.ResolveMacroFunction(tokens);
        var expression = new CConstantExpression(result);

        Assert.Equal(expectedType, expression.GetTypeOfExpression());
        Assert.Equal($"( ({castType}) ( 1 + 2 ) ) * 3", CSConstantExpression.FromCConstantExpression(expression).ToString());
        Assert.All(result, token => Assert.Equal(sourceLocation, token.SourceLocation));
        Assert.Same(tokens[2], result[3]);
        Assert.Same(tokens[3], result[4]);
        Assert.Same(tokens[4], result[5]);
    }

    [Theory]
    [InlineData("INT64_C")]
    [InlineData("UINT64_C")]
    [InlineData("UINT32_C")]
    public void IntegerMacroPreservesIdentifierArgumentsForLaterResolution(string name)
    {
        var identifier = new CConstIdentifierToken("VALUE", new CppSourceLocation("constants.h", 0, 1, 1));

        var handler = AllBuiltinMacroFunctions.Functions.Single(handler => handler.Name == name);
        Assert.True(handler.TryEvaluate([[identifier]], out var result));
        Assert.NotNull(result);
        Assert.Same(identifier, result[3]);
    }

    [Theory]
    [InlineData("UINT64_C", "18446744073709551616")]
    [InlineData("UINT64_C", "-1")]
    [InlineData("UINT32_C", "4294967296")]
    [InlineData("UINT32_C", "-1")]
    public void IntegerMacroRejectsOutOfRangeUnsignedLiteral(string name, string value)
    {
        var literal = new CConstLiteralToken(value, CConstantType.UnsignedLongLong, new CppSourceLocation("constants.h", 0, 1, 1));
        var handler = AllBuiltinMacroFunctions.Functions.Single(handler => handler.Name == name);

        Assert.False(handler.TryEvaluate([[literal]], out var result));
        Assert.Null(result);
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
