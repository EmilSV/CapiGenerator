using System.Globalization;
using CapiGenerator.ConstantToken;
using CapiGenerator.Model;

namespace CapiGenerator;

public class DefaultConstTypeResolver : IConstantTypeResolver
{
    private static bool IsValidIntPunctuation(ConstantPunctuationToken token) => token.Type switch
    {
        PunctuationType.Plus => true,
        PunctuationType.Minus => true,
        PunctuationType.Multiply => true,
        PunctuationType.Divide => true,
        PunctuationType.Modulo => true,
        PunctuationType.BitwiseAnd => true,
        PunctuationType.BitwiseOr => true,
        PunctuationType.BitwiseXor => true,
        PunctuationType.BitwiseNot => true,
        PunctuationType.BitwiseLeftShift => true,
        PunctuationType.BitwiseRightShift => true,
        PunctuationType.LeftParenthesis => true,
        PunctuationType.RightParenthesis => true,
        _ => false,
    };

    private static bool IsValidFloatPunctuation(ConstantPunctuationToken token) => token.Type switch
    {
        PunctuationType.Plus => true,
        PunctuationType.Minus => true,
        PunctuationType.Multiply => true,
        PunctuationType.Divide => true,
        PunctuationType.Modulo => true,
        PunctuationType.LeftParenthesis => true,
        PunctuationType.RightParenthesis => true,
        _ => false,
    };

    private static bool IsValidStringPunctuation(ConstantPunctuationToken token) => token.Type switch
    {
        PunctuationType.Plus => true,
        PunctuationType.LeftParenthesis => true,
        PunctuationType.RightParenthesis => true,
        _ => false,
    };

    private static bool IsValidCharPunctuation(ConstantPunctuationToken token) => token.Type switch
    {
        PunctuationType.LeftParenthesis => true,
        PunctuationType.RightParenthesis => true,
        _ => false,
    };


    public ConstantType ResolveType(Constant constant)
    {
        return InnerResolveType(constant, new List<Constant>());
    }

    private ConstantType InnerResolveType(Constant constant, List<Constant> visitedConstant)
    {
        if (visitedConstant.Contains(constant))
        {
            var firstConstant = visitedConstant[0];
            throw new ConstRecursiveResolutionException(visitedConstant.ToArray(), $"Circular reference detected for constant {firstConstant.Output.Name}");
        }
        visitedConstant.Add(constant);

        if (IsIntType(constant, visitedConstant))
        {
            return ConstantType.Int;
        }
        else if (IsFloatType(constant, visitedConstant))
        {
            return ConstantType.Float;
        }
        else if (IsStringType(constant, visitedConstant))
        {
            return ConstantType.String;
        }
        else if (IsCharType(constant, visitedConstant))
        {
            return ConstantType.Char;
        }
        else
        {
            return ConstantType.Unknown;
        }
    }

    private bool IsStringType(Constant constant, List<Constant> visitedConstant)
    {
        foreach (var token in constant.Output.Tokens)
        {
            if (token is ConstantPunctuationToken punctuationToken)
            {
                if (!IsValidStringPunctuation(punctuationToken))
                {
                    return false;
                }
            }
            else if (token is ConstantLiteralToken literalToken)
            {
                if (literalToken.Type != ConstantType.String)
                {
                    return false;
                }
            }
            else if (token is ConstIdentifierToken identifierToken)
            {
                var referencedConstant = identifierToken.Value.Get(constant.OwingFactory);
                if (referencedConstant == null)
                {
                    return false;
                }

                var referencedConstantType = InnerResolveType(referencedConstant, visitedConstant);
                if (referencedConstantType is ConstantType.Int or ConstantType.Float or ConstantType.String)
                {
                    return false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        return true;
    }

    private bool IsFloatType(Constant constant, List<Constant> visitedConstant)
    {
        foreach (var token in constant.Output.Tokens)
        {
            if (token is ConstantPunctuationToken punctuationToken)
            {
                if (!IsValidFloatPunctuation(punctuationToken))
                {
                    return false;
                }
            }
            else if (token is ConstantLiteralToken literalToken)
            {
                if (literalToken.Type is not ConstantType.Float and not ConstantType.Int)
                {
                    return false;
                }
            }
            else if (token is ConstIdentifierToken identifierToken)
            {
                var referencedConstant = identifierToken.Value.Get(constant.OwingFactory);
                if (referencedConstant == null)
                {
                    return false;
                }

                var referencedConstantType = InnerResolveType(referencedConstant, visitedConstant);
                if (referencedConstantType is not ConstantType.Int and not ConstantType.Float)
                {
                    return false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        return true;
    }

    private bool IsIntType(Constant constant, List<Constant> visitedConstant)
    {
        foreach (var token in constant.Output.Tokens)
        {
            if (token is ConstantPunctuationToken punctuationToken)
            {
                if (!IsValidIntPunctuation(punctuationToken))
                {
                    return false;
                }
            }
            else if (token is ConstantLiteralToken literalToken)
            {
                if (literalToken.Type != ConstantType.Int)
                {
                    return false;
                }
            }
            else if (token is ConstIdentifierToken identifierToken)
            {
                var referencedConstant = identifierToken.Value.Get(constant.OwingFactory);
                if (referencedConstant == null)
                {
                    return false;
                }

                var referencedConstantType = InnerResolveType(referencedConstant, visitedConstant);
                if (referencedConstantType != ConstantType.Int)
                {
                    return false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        return true;
    }

    private bool IsCharType(Constant constant, List<Constant> visitedConstant)
    {
        foreach (var token in constant.Output.Tokens)
        {
            if (token is ConstantPunctuationToken punctuationToken)
            {
                if (!IsValidCharPunctuation(punctuationToken))
                {
                    return false;
                }
            }
            else if (token is ConstantLiteralToken literalToken)
            {
                if (literalToken.Type != ConstantType.Char)
                {
                    return false;
                }
            }
            else if (token is ConstIdentifierToken identifierToken)
            {
                var referencedConstant = identifierToken.Value.Get(constant.OwingFactory);
                if (referencedConstant == null)
                {
                    return false;
                }

                var referencedConstantType = InnerResolveType(referencedConstant, visitedConstant);
                if (referencedConstantType != ConstantType.Char)
                {
                    return false;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        return true;
    }
}
