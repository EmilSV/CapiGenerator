namespace CapiGenerator.CSModel;


public abstract class CSBaseType(string name)
    : BaseCSAstItem, ICSType
{
    public string Name => name;
    public abstract string FullName { get; }
}