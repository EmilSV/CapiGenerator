namespace CapiGenerator.CSModel;

public class CSEnum(
    string name,
    CSResolveType? type,
    ReadOnlySpan<CSEnumValue> values
) : CSBaseType(name)
{
    private readonly CSEnumValue[] _values = values.ToArray();

    public CSResolveType? Type => type;
    public ReadOnlySpan<CSEnumValue> Values => _values;
}