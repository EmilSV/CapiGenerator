using System.Collections;
using System.Diagnostics;
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

    public bool IsResolved()
    {
        return IsResolved([]);
    }

    internal bool IsResolved(HashSet<CConstant> visitedConstants)
    {
        if (_tokens.OfType<CConstCastToken>().Any(token => !token.TryGetConstantType(out _)))
        {
            return false;
        }

        foreach (var identifierToken in _tokens.OfType<CConstIdentifierToken>())
        {
            if (identifierToken.GetConstantModel() is not { } constantModel)
            {
                return false;
            }

            if (constantModel is CConstant constant)
            {
                if (!visitedConstants.Add(constant))
                {
                    return false;
                }

                var isResolved = constant.Expression.IsResolved(visitedConstants);
                visitedConstants.Remove(constant);
                if (!isResolved)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public CConstantType GetTypeOfExpression()
    {
        if (_constantType != CConstantType.NONE)
        {
            return _constantType;
        }

        static CConstantType GetConstantType(CConstantType currentConstantType, CConstantType tokenType)
        {
            if ((int)tokenType < 0)
            {
                return tokenType;
            }

            return (CConstantType)Math.Max((int)currentConstantType, (int)tokenType);
        }

        CConstantType constantType = CConstantType.NONE;
        CConstantType castType = CConstantType.NONE;
        foreach (var token in _tokens)
        {
            if (token is CConstLiteralToken literalToken)
            {
                constantType = GetConstantType(constantType, literalToken.Type);
            }
            else if (token is CConstCastToken castToken)
            {
                if (!castToken.TryGetConstantType(out castType))
                {
                    throw new InvalidOperationException(
                        $"Cast type not resolved at {castToken.SourceLocation.File} " +
                        $"{castToken.SourceLocation.Line}: {castToken.SourceLocation.Column}");
                }
            }
            else if (token is CConstIdentifierToken identifierToken)
            {
                var identifierTokenConstant = identifierToken.GetConstantModel();
                if (identifierTokenConstant == null)
                {
                    Debugger.Break();
                    throw new InvalidOperationException($"Constant {(identifierToken.TryGetName(out var name) ? name : "unknown")} not resolve at {identifierToken.SourceLocation.File} {identifierToken.SourceLocation.Line}: {identifierToken.SourceLocation.Column}");
                }
                constantType = GetConstantType(constantType, identifierTokenConstant.GetCConstantType());
            }

            if (constantType is CConstantType.Unknown)
            {
                break;
            }
        }

        _constantType = castType != CConstantType.NONE ? castType : constantType;

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
