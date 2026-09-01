using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CSModel.ConstantToken;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

[CollectionBuilder(typeof(CSConstantExpressionBuilder), "Create")]
public sealed class CSConstantExpression(ReadOnlySpan<BaseCSConstantToken> tokens) : IEnumerable<BaseCSConstantToken>
{
    public static class CSConstantExpressionBuilder
    {
        public static CSConstantExpression Create(ReadOnlySpan<BaseCSConstantToken> tokens)
        {
            return new CSConstantExpression(tokens);
        }
    }

    private readonly BaseCSConstantToken[] _tokens = NormalizeUnsignedNegativeCasts(tokens);
    public ReadOnlySpan<BaseCSConstantToken> Tokens => _tokens;

    public int Count => _tokens.Length;


    public void OnSecondPass(CSTranslationUnit compilationUnit)
    {
        foreach (var token in _tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }

    public BaseCSConstantToken this[int index] => _tokens[index];

    public IEnumerator<BaseCSConstantToken> GetEnumerator()
    {
        return ((IEnumerable<BaseCSConstantToken>)_tokens).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _tokens.GetEnumerator();
    }


    public static CSConstantExpression FromCConstantExpression(CConstantExpression expression)
    {
        List<BaseCSConstantToken> tokens = [];
        AddTokens(expression, tokens, []);
        MergeAdjacentUtf8Literals(tokens);
        AppendNullTerminatorToUtf8Literals(tokens);
        return new CSConstantExpression(tokens.ToArray());
    }

    private static BaseCSConstantToken[] NormalizeUnsignedNegativeCasts(ReadOnlySpan<BaseCSConstantToken> sourceTokens)
    {
        List<BaseCSConstantToken> outTokens = [];
        int i = 0;
        for (; i + 2 < sourceTokens.Length; i++)
        {
            if (sourceTokens[i] is not CSConstCastToken castToken ||
                sourceTokens[i + 1] is not CSConstantPunctuationToken { Type: CSPunctuationType.Minus } ||
                sourceTokens[i + 2] is not CSConstLiteralToken literalToken ||
                !literalToken.TryParseValueAsInteger(out var integerValue) ||
                integerValue <= 0 ||
                !castToken.IsUnsignedIntegerCast)
            {
                outTokens.Add(sourceTokens[i]);
                continue;
            }

            outTokens.Add(new CSConstUncheckedToken());
            outTokens.Add(new CSConstantPunctuationToken { Type = CSPunctuationType.LeftParenthesis });
            outTokens.Add(castToken);
            if (castToken.IsNativeUnsignedIntegerCast)
            {
                outTokens.Add(new CSConstantPunctuationToken { Type = CSPunctuationType.LeftParenthesis });
            }
            outTokens.Add(new CSConstantPunctuationToken { Type = CSPunctuationType.Minus });
            outTokens.Add(literalToken);
            if (castToken.IsNativeUnsignedIntegerCast)
            {
                outTokens.Add(new CSConstantPunctuationToken { Type = CSPunctuationType.RightParenthesis });
            }
            outTokens.Add(new CSConstantPunctuationToken { Type = CSPunctuationType.RightParenthesis });

            i += 2;
        }
        for (; i < sourceTokens.Length; i++)
        {
            outTokens.Add(sourceTokens[i]);
        }


        return outTokens.ToArray();
    }

    private static void MergeAdjacentUtf8Literals(List<BaseCSConstantToken> tokens)
    {
        for (int i = tokens.Count - 1; i > 0; i--)
        {
            if (tokens[i - 1] is not CSConstLiteralToken left ||
                tokens[i] is not CSConstLiteralToken right ||
                !left.Utf8Literal ||
                !right.Utf8Literal ||
                left.Value.Length < 2 ||
                right.Value.Length < 2)
            {
                continue;
            }

            left.Value = $"{left.Value[..^1]}{right.Value[1..]}";
            tokens.RemoveAt(i);
        }
    }

    private static void AppendNullTerminatorToUtf8Literals(List<BaseCSConstantToken> tokens)
    {
        for (int i = 0; i < tokens.Count; i++)
        {
            if (tokens[i] is CSConstLiteralToken { Utf8Literal: true } token && token.Value.Length > 0 && !token.Value.EndsWith('\0'))
            {
                tokens[i] = new CSConstLiteralToken($"{token.Value}\0", token.Type);
            }
        }
    }

    private static void AddTokens(
        CConstantExpression expression,
        List<BaseCSConstantToken> output,
        HashSet<CConstant> visitedConstants)
    {
        foreach (var token in expression.Tokens)
        {
            if (token is CConstIdentifierToken { } identifierToken &&
                identifierToken.GetConstantModel() is CConstant { } constant &&
                constant.GetCConstantType() == CConstantType.String &&
                visitedConstants.Add(constant))
            {
                AddTokens(constant.Expression, output, visitedConstants);
                visitedConstants.Remove(constant);
                continue;
            }

            output.Add(CConstantTokenToCSConstantToken(token));
        }
    }

    private static BaseCSConstantToken CConstantTokenToCSConstantToken(BaseCConstantToken token)
    {
        return token switch
        {
            CConstantPunctuationToken punctuationToken => CSConstantPunctuationToken.FromCConstantPunctuationToken(punctuationToken),
            CConstLiteralToken literalToken => CSConstLiteralToken.FromCConstantLiteralToken(literalToken),
            CConstCastToken castToken when castToken.TryGetConstantType(out var castType) => new CSConstCastToken(castType),
            CConstCastToken castToken => throw new InvalidOperationException(
                $"Cast type not resolved at {castToken.SourceLocation.File} " +
                $"{castToken.SourceLocation.Line}: {castToken.SourceLocation.Column}"),
            CConstIdentifierToken identifierToken => CSConstIdentifierToken.FromCConstantToken(identifierToken),
            _ => throw new NotImplementedException()
        };
    }

    public override string ToString()
    {
        if (_tokens.Length == 0)
            return string.Empty;

        var builder = new StringBuilder();
        foreach (var token in _tokens[..^1])
        {
            builder.Append(token);
            builder.Append(' ');
        }
        builder.Append(_tokens[^1]);
        return builder.ToString();
    }
}
