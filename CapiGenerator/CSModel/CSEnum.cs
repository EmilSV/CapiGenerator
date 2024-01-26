namespace CapiGenerator.CSModel;

public class CSEnum(
    string name,
    CSResolveType type,
    CSDefaultValue defaultValue,
    ReadOnlySpan<CSField> values
) : CSBaseType(name)
{
    private readonly CSField[] _values = values.ToArray();

    public CSResolveType Type => type;
    public CSDefaultValue DefaultValue => defaultValue;
    public ReadOnlySpan<CSField> Values => _values;
}