using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.CModel.ConstantToken;


public sealed class CConstantPunctuationToken : BaseCConstantToken
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

    public static CConstantPunctuationToken Parse(string value)
    {
        if (TryParse(value, out var constantPunctuation))
        {
            return constantPunctuation;
        }

        throw new ArgumentException(value, nameof(value), null);
    }

    public static bool TryParse(string value, [NotNullWhen(true)] out CConstantPunctuationToken? constantPunctuation)
    {
        constantPunctuation = value switch
        {
            "+" => new() { Type = CPunctuationType.Plus },
            "-" => new() { Type = CPunctuationType.Minus },
            "*" => new() { Type = CPunctuationType.Multiply },
            "/" => new() { Type = CPunctuationType.Divide },
            "%" => new() { Type = CPunctuationType.Modulo },
            "&" => new() { Type = CPunctuationType.BitwiseAnd },
            "|" => new() { Type = CPunctuationType.BitwiseOr },
            "^" => new() { Type = CPunctuationType.BitwiseXor },
            "~" => new() { Type = CPunctuationType.BitwiseNot },
            "<<" => new() { Type = CPunctuationType.BitwiseLeftShift },
            ">>" => new() { Type = CPunctuationType.BitwiseRightShift },
            "(" => new() { Type = CPunctuationType.LeftParenthesis },
            ")" => new() { Type = CPunctuationType.RightParenthesis },
            _ => null
        };

        return constantPunctuation is not null;
    }
}