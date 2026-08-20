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

    private readonly BaseCSConstantToken[] _tokens = tokens.ToArray();
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
        return new CSConstantExpression(tokens.ToArray());
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