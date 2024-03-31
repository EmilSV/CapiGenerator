using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;


public abstract class CSBaseType(string name)
    : BaseCSAstItem, ICSType
{
    public HistoricValue<string> Name = new(name);
    public abstract ComputedValue<string> FullName { get; }
}