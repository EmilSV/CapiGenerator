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
        var tokens = new BaseCSConstantToken[expression.Tokens.Length];
        for (int i = 0; i < expression.Tokens.Length; i++)
        {
            tokens[i] = CConstantTokenToCSConstantToken(expression.Tokens[i]);
        }
        return new CSConstantExpression(tokens);
    }

    private static BaseCSConstantToken CConstantTokenToCSConstantToken(BaseCConstantToken token)
    {
        return token switch
        {
            CConstantPunctuationToken punctuationToken => CSConstantPunctuationToken.FromCConstantPunctuationToken(punctuationToken),
            CConstLiteralToken literalToken => CSConstLiteralToken.FromCConstantLiteralToken(literalToken),
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