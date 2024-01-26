namespace CapiGenerator.CSModel;

public class CSParameter(
    string name,
    CSResolveType type,
    CSDefaultValue defaultValue
):BaseCSAstItem
{
    public string Name => name;
    public CSResolveType Type => type;
    public CSDefaultValue DefaultValue => defaultValue;
}