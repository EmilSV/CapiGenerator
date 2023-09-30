using CapiGenerator.Model;

namespace CapiGenerator.ConstantToken;

public record class ConstIdentifierToken : BaseConstantToken
{
    public required GuidRef<Constant> Value { get; init; }

    public override string GetOutValue()
    {
        var constant = Value.Get() ?? throw new InvalidOperationException($"Constant {Value} not found");
        return constant.Output.Name;
    }
}