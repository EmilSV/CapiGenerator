using CapiGenerator.CModel;

namespace CapiGenerator.CModel.Type;

public abstract class BaseCAnonymousType(Guid compilationUnitId)
    : BaseCAstItem(compilationUnitId), ICType
{
    public string? Name => null;

    public abstract bool GetIsCompletedType();

    public bool IsAnonymous => true;
}