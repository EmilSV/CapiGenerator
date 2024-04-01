using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;


public abstract class CSBaseType(string name)
    : BaseCSAstItem, ICSType
{
    public HistoricValue<string> Name = new(name);
    public abstract ComputedValueOrValue<string> FullName { get; }
    string ICSType.FullName => FullName.Value;
}