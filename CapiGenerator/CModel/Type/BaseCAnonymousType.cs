using CapiGenerator.CModel;

namespace CapiGenerator.CModel.Type;

public abstract class BaseCAnonymousType
    : BaseCAstItem, ICType
{
    protected BaseCAnonymousType()
    {
    }

    protected BaseCAnonymousType(object primarySource)
        : base(primarySource)
    {
    }

    public string? Name => null;

    public abstract bool GetIsCompletedType();

    public bool IsAnonymous => true;
}