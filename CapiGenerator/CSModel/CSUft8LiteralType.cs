using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSUft8LiteralType : CSBaseType
{
    private CSUft8LiteralType()
        : base("ReadonlySpan<byte>")
    {
        this.Name = HistoricValue<string>.NewReadOnly("ReadonlySpan<byte>");
    }

    public override ComputedValueOrValue<string> FullName => "ReadonlySpan<byte>";

    public static CSUft8LiteralType Instance { get; } = new();
}