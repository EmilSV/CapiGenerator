using CapiGenerator.Model;

namespace CapiGenerator.Type;

public abstract class BaseAnonymousType(Guid compilationUnitId) 
    : BaseCAstItem(compilationUnitId)
{
    public abstract bool GetIsCompletedType();
}