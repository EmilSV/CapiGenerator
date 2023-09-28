using CapiGenerator.Model;

namespace CapiGenerator.ConstantToken;

public record class ConstantLiteralToken : BaseConstantToken
{
    public enum LiteralType
    {
        String,
        Integer,
        Float,
    }

    public required string Value { get; init; }
    public required LiteralType Type { get; init; }

    public override string GetValue(IReadOnlyDictionary<Guid, Constant> constants)
    {
        return Value;
    }
}