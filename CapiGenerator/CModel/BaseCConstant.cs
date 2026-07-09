namespace CapiGenerator.CModel;

public abstract class BaseCConstant :
    BaseCAstItem, ICConstAssignable
{
    protected BaseCConstant()
    {
    }

    protected BaseCConstant(object primarySource)
        : base(primarySource)
    {
    }

    public abstract string Name { get; }
    public abstract CConstantType GetCConstantType();
}