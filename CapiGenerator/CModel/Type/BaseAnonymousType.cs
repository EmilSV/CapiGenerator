using CapiGenerator.CModel;

namespace CapiGenerator.CModel.Type;

public abstract class BaseAnonymousType(Guid compilationUnitId) 
    : BaseCAstItem(compilationUnitId)
{
    public abstract bool GetIsCompletedType();
}