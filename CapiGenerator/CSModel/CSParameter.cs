namespace CapiGenerator.CSModel;

public class CSParameter(
    string name,
    CSResolveType type,
    CSDefaultValue defaultValue
)
{
    public string Name => name;
    public CSResolveType Type => type;
    public CSDefaultValue DefaultValue => defaultValue;
}