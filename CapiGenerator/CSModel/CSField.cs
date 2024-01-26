namespace CapiGenerator.CSModel;

public class CSField(
    string name,
    CSResolveType type,
    CSDefaultValue defaultValue
) : BaseCSAstItem
{
    public string Name => name;
    public CSResolveType Type => type;
    public CSDefaultValue DefaultValue => defaultValue;
}