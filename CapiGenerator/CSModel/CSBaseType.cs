namespace CapiGenerator.CSModel;


public abstract class CSBaseType(string name) : BaseCSAstItem
{
    public string Name => name;
}