using CapiGenerator.Model;

namespace CapiGenerator.ConstantToken;

public record class ConstIdentifierToken : BaseConstantToken
{
    public required GuidRef<Constant> Value { get; init; }

    public override string GetValue(IReadOnlyDictionary<Guid, Constant> constants)
    {
        return constants[Value.Guid].Output.Name;
    }
}