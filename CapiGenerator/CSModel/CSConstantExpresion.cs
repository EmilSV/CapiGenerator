using System.Collections;
using System.Runtime.CompilerServices;
using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CSModel.ConstantToken;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

[CollectionBuilder(typeof(CSConstantExpressionBuilder), "Create")]
public sealed class CSConstantExpression(ReadOnlySpan<BaseCSConstantToken> tokens) : IEnumerable<BaseCSConstantToken>
{
    internal static class CSConstantExpressionBuilder
    {
        internal static CSConstantExpression Create(ReadOnlySpan<BaseCSConstantToken> tokens)
        {
            return new CSConstantExpression(tokens);
        }
    }

    private readonly BaseCSConstantToken[] _tokens = tokens.ToArray();
    private CSConstantType _constantType = CSConstantType.NONE;
    public ReadOnlySpan<BaseCSConstantToken> Tokens => _tokens;


    public void OnSecondPass(CSTranslationUnit compilationUnit)
    {
        foreach (var token in _tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }

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
}