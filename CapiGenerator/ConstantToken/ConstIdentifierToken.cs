using CapiGenerator.Model;

namespace CapiGenerator.ConstantToken;

public record class ConstIdentifierToken : BaseConstantToken
{
    public required ModelRef<Constant> Value { get; init; }

    public override string GetOutValue(BaseModelRefLookup<Constant> constLookup)
    {
        var constant = constLookup.Get(Value) ?? throw new InvalidOperationException($"Constant {Value} not found");
        return constant.Output.Name;
    }
}