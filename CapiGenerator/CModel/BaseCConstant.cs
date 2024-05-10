namespace CapiGenerator.CModel;

public abstract class BaseCConstant :
    BaseCAstItem, ICConstAssignable
{
    public abstract string Name { get; }
    public abstract CConstantType GetCConstantType();
}