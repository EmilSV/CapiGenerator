using System.Diagnostics.CodeAnalysis;
using CapiGenerator.Model;

namespace CapiGenerator.ConstantToken;

public enum PunctuationType
{
    Plus,
    Minus,
    Multiply,
    Divide,
    Modulo,
    BitwiseAnd,
    BitwiseOr,
    BitwiseXor,
    BitwiseNot,
    BitwiseLeftShift,
    BitwiseRightShift,
    LeftParenthesis,
    RightParenthesis,
}


public class ConstantPunctuationToken : BaseConstantToken
{
    public required PunctuationType Type { get; init; }

    public override string ToString() => Type switch
    {
        PunctuationType.Plus => "+",
        PunctuationType.Minus => "-",
        PunctuationType.Multiply => "*",
        PunctuationType.Divide => "/",
        PunctuationType.Modulo => "%",
        PunctuationType.BitwiseAnd => "&",
        PunctuationType.BitwiseOr => "|",
        PunctuationType.BitwiseXor => "^",
        PunctuationType.BitwiseNot => "~",
        PunctuationType.BitwiseLeftShift => "<<",
        PunctuationType.BitwiseRightShift => ">>",
        PunctuationType.LeftParenthesis => "(",
        PunctuationType.RightParenthesis => ")",
        _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
    };

    public static ConstantPunctuationToken Parse(string value)
    {
        if (TryParse(value, out var constantPunctuation))
        {
            return constantPunctuation;
        }

        throw new ArgumentException(nameof(value), value, null);
    }

    public static bool TryParse(string value, [NotNullWhen(true)] out ConstantPunctuationToken? constantPunctuation)
    {
        constantPunctuation = value switch
        {
            "+" => new() { Type = PunctuationType.Plus },
            "-" => new() { Type = PunctuationType.Minus },
            "*" => new() { Type = PunctuationType.Multiply },
            "/" => new() { Type = PunctuationType.Divide },
            "%" => new() { Type = PunctuationType.Modulo },
            "&" => new() { Type = PunctuationType.BitwiseAnd },
            "|" => new() { Type = PunctuationType.BitwiseOr },
            "^" => new() { Type = PunctuationType.BitwiseXor },
            "~" => new() { Type = PunctuationType.BitwiseNot },
            "<<" => new() { Type = PunctuationType.BitwiseLeftShift },
            ">>" => new() { Type = PunctuationType.BitwiseRightShift },
            "(" => new() { Type = PunctuationType.LeftParenthesis },
            ")" => new() { Type = PunctuationType.RightParenthesis },
            _ => null
        };

        return constantPunctuation is not null;
    }

    public override string GetOutValue()
    {
        return ToString();
    }
}