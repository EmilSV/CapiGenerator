using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSBannedType : CSBaseType
{
    private CSBannedType(string name) : base(name)
    {
        this.Name = HistoricValue<string>.NewReadOnly(name);
    }

    public static CSBannedType Instance { get; } = new CSBannedType("__BanedType__");

    public override ComputedValueOrValue<string> FullName => Name;
}