namespace CapiGenerator.CSModel;

public abstract class BaseCSAnonymousType()
    : BaseCSAstItem(), ICSType
{
    public virtual string? FullName => null;
}