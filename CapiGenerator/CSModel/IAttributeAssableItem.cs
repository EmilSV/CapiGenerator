using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public interface IAttributeAssignableItem
{
    public NotifyList<BaseCSAttribute> Attributes { get; }
}