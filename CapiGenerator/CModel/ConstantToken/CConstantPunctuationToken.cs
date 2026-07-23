using System.Diagnostics.CodeAnalysis;
using CppAst;

namespace CapiGenerator.CModel.ConstantToken;


public sealed class CConstantPunctuationToken(CppSourceLocation debugInfo) : BaseCConstantToken(debugInfo)
{
    public required CPunctuationType Type { get; init; }

    public override string ToString() => Type switch
    {
        CPunctuationType.Plus => "+",
        CPunctuationType.Minus => "-",
        CPunctuationType.Multiply => "*",
        CPunctuationType.Divide => "/",
        CPunctuationType.Modulo => "%",
        CPunctuationType.BitwiseAnd => "&",
        CPunctuationType.BitwiseOr => "|",
        CPunctuationType.BitwiseXor => "^",
        CPunctuationType.BitwiseNot => "~",
        CPunctuationType.BitwiseLeftShift => "<<",
        CPunctuationType.BitwiseRightShift => ">>",
        CPunctuationType.LeftParenthesis => "(",
        CPunctuationType.RightParenthesis => ")",
        _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
    };

    public static CConstantPunctuationToken Parse(string value, CppSourceLocation debugInfo)
    {
        if (TryParse(value, debugInfo, out var constantPunctuation))
        {
            return constantPunctuation;
        }

        throw new ArgumentException(value, nameof(value), null);
    }

    public static bool TryParse(
        string value,
        CppSourceLocation debugInfo,
        [NotNullWhen(true)] out CConstantPunctuationToken? constantPunctuation)
    {
        constantPunctuation = value switch
        {
            "+" => new(debugInfo) { Type = CPunctuationType.Plus },
            "-" => new(debugInfo) { Type = CPunctuationType.Minus },
            "*" => new(debugInfo) { Type = CPunctuationType.Multiply },
            "/" => new(debugInfo) { Type = CPunctuationType.Divide },
            "%" => new(debugInfo) { Type = CPunctuationType.Modulo },
            "&" => new(debugInfo) { Type = CPunctuationType.BitwiseAnd },
            "|" => new(debugInfo) { Type = CPunctuationType.BitwiseOr },
            "^" => new(debugInfo) { Type = CPunctuationType.BitwiseXor },
            "~" => new(debugInfo) { Type = CPunctuationType.BitwiseNot },
            "<<" => new(debugInfo) { Type = CPunctuationType.BitwiseLeftShift },
            ">>" => new(debugInfo) { Type = CPunctuationType.BitwiseRightShift },
            "(" => new(debugInfo) { Type = CPunctuationType.LeftParenthesis },
            ")" => new(debugInfo) { Type = CPunctuationType.RightParenthesis },
            _ => null
        };

        return constantPunctuation is not null;
    }
}