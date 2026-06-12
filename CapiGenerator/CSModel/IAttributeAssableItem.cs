using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public interface IAttributeAssignableItem
{
    public List<BaseCSAttribute> Attributes { get; }
}
