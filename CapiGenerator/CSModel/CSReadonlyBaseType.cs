using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class CSReadonlyBaseType :
   BaseCSAstItem, ICSType
{
    public abstract string? Namespace { get; }
    public abstract string Name { get; }
    public new abstract InstanceId Id { get; }
}