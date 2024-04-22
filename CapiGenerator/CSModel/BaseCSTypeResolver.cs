using CapiGenerator.CModel;

namespace CapiGenerator.CSModel;

public abstract class BaseCSTypeResolver
{
    public abstract BaseCSType Resolve(ICType type);
    public abstract bool IsResolved(ICType type); 
}