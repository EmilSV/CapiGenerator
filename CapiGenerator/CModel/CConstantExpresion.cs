using System.Collections;
using System.Runtime.CompilerServices;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Parser;

namespace CapiGenerator.CModel;


[CollectionBuilder(typeof(CConstantExpressionBuilder), nameof(CConstantExpressionBuilder.Create))]
public sealed class CConstantExpression(ReadOnlySpan<BaseCConstantToken> tokens) : IEnumerable<BaseCConstantToken>
{
    internal static class CConstantExpressionBuilder
    {
        internal static CConstantExpression Create(ReadOnlySpan<BaseCConstantToken> tokens)
        {
            return new CConstantExpression(tokens);
        }
    }

    private readonly BaseCConstantToken[] _tokens = tokens.ToArray();
    private CConstantType _constantType = CConstantType.NONE;
    public ReadOnlySpan<BaseCConstantToken> Tokens => _tokens;

    public CConstantType GetTypeOfExpression()
    {
        if (_constantType != CConstantType.NONE)
        {
            return _constantType;
        }

        static CConstantType GetConstantType(CConstantType currentConstantType, CConstantType tokenType) =>
            tokenType switch
            {
                CConstantType.UnsignedInt
                    when currentConstantType is CConstantType.NONE or CConstantType.UnsignedInt =>
                        CConstantType.UnsignedInt,
                CConstantType.Int
                    when currentConstantType is CConstantType.NONE or CConstantType.Int or CConstantType.UnsignedInt =>
                        CConstantType.Int,
                CConstantType.Float or CConstantType.Int
                    when currentConstantType is CConstantType.NONE or CConstantType.Float or CConstantType.Int =>
                        CConstantType.Float,
                CConstantType.Char
                    when currentConstantType is CConstantType.NONE or CConstantType.Char =>
                    CConstantType.Char,

                CConstantType.String or CConstantType.Char
                    when currentConstantType is CConstantType.NONE or CConstantType.String or CConstantType.Char =>
                        CConstantType.String,

                _ => CConstantType.Unknown
            };

        CConstantType constantType = CConstantType.NONE;
        foreach (var token in _tokens)
        {
            if (token is CConstLiteralToken literalToken)
            {
                constantType = GetConstantType(constantType, literalToken.Type);
            }
            else if (token is CConstIdentifierToken identifierToken)
            {
                var identifierTokenConstant = identifierToken.GetConstantModel() ??
                    throw new InvalidOperationException($"Constant {identifierToken} not resolve");
                constantType = GetConstantType(constantType, identifierTokenConstant.Expression.GetTypeOfExpression());
            }

            if (constantType is CConstantType.Unknown)
            {
                break;
            }
        }

        _constantType = constantType;

        return _constantType;
    }

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in _tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }

    public IEnumerator<BaseCConstantToken> GetEnumerator()
    {
        return ((IEnumerable<BaseCConstantToken>)_tokens).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _tokens.GetEnumerator();
    }
}
