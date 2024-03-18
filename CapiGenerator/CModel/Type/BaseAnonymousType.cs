using CapiGenerator.CModel;

namespace CapiGenerator.CModel.Type;

public abstract class BaseCSAnonymousType(Guid compilationUnitId)
    : BaseCAstItem(compilationUnitId), ICType
{
    public string Name => "__anonymous_type__";

    public abstract bool GetIsCompletedType();
}