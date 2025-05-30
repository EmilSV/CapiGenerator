using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;

namespace CapiGenerator.CSModel.ConstantToken;

public sealed class CSConstantPunctuationToken : BaseCSConstantToken
{
    public required CSPunctuationType Type { get; init; }

    public override string ToString() => Type switch
    {
        CSPunctuationType.Plus => "+",
        CSPunctuationType.Minus => "-",
        CSPunctuationType.Multiply => "*",
        CSPunctuationType.Divide => "/",
        CSPunctuationType.Modulo => "%",
        CSPunctuationType.BitwiseAnd => "&",
        CSPunctuationType.BitwiseOr => "|",
        CSPunctuationType.BitwiseXor => "^",
        CSPunctuationType.BitwiseNot => "~",
        CSPunctuationType.BitwiseLeftShift => "<<",
        CSPunctuationType.BitwiseRightShift => ">>",
        CSPunctuationType.LeftParenthesis => "(",
        CSPunctuationType.RightParenthesis => ")",
        _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, null)
    };

    public static CSConstantPunctuationToken Parse(string value)
    {
        if (TryParse(value, out var constantPunctuation))
        {
            return constantPunctuation;
        }

        throw new ArgumentException(value, nameof(value), null);
    }

    public static bool TryParse(string value, [NotNullWhen(true)] out CSConstantPunctuationToken? constantPunctuation)
    {
        constantPunctuation = value switch
        {
            "+" => new() { Type = CSPunctuationType.Plus },
            "-" => new() { Type = CSPunctuationType.Minus },
            "*" => new() { Type = CSPunctuationType.Multiply },
            "/" => new() { Type = CSPunctuationType.Divide },
            "%" => new() { Type = CSPunctuationType.Modulo },
            "&" => new() { Type = CSPunctuationType.BitwiseAnd },
            "|" => new() { Type = CSPunctuationType.BitwiseOr },
            "^" => new() { Type = CSPunctuationType.BitwiseXor },
            "~" => new() { Type = CSPunctuationType.BitwiseNot },
            "<<" => new() { Type = CSPunctuationType.BitwiseLeftShift },
            ">>" => new() { Type = CSPunctuationType.BitwiseRightShift },
            "(" => new() { Type = CSPunctuationType.LeftParenthesis },
            ")" => new() { Type = CSPunctuationType.RightParenthesis },
            _ => null
        };

        return constantPunctuation is not null;
    }


    public static CSConstantPunctuationToken FromCConstantPunctuationToken(CConstantPunctuationToken token) =>
        token.Type switch
        {
            CPunctuationType.Plus => new() { Type = CSPunctuationType.Plus },
            CPunctuationType.Minus => new() { Type = CSPunctuationType.Minus },
            CPunctuationType.Multiply => new() { Type = CSPunctuationType.Multiply },
            CPunctuationType.Divide => new() { Type = CSPunctuationType.Divide },
            CPunctuationType.Modulo => new() { Type = CSPunctuationType.Modulo },
            CPunctuationType.BitwiseAnd => new() { Type = CSPunctuationType.BitwiseAnd },
            CPunctuationType.BitwiseOr => new() { Type = CSPunctuationType.BitwiseOr },
            CPunctuationType.BitwiseXor => new() { Type = CSPunctuationType.BitwiseXor },
            CPunctuationType.BitwiseNot => new() { Type = CSPunctuationType.BitwiseNot },
            CPunctuationType.BitwiseLeftShift => new() { Type = CSPunctuationType.BitwiseLeftShift },
            CPunctuationType.BitwiseRightShift => new() { Type = CSPunctuationType.BitwiseRightShift },
            CPunctuationType.LeftParenthesis => new() { Type = CSPunctuationType.LeftParenthesis },
            CPunctuationType.RightParenthesis => new() { Type = CSPunctuationType.RightParenthesis },
            _ => throw new ArgumentOutOfRangeException(nameof(token), token.Type, null)
        };
}