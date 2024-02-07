using CapiGenerator.CModel;

namespace CapiGenerator.CSModel;

public class CSField(
    string name,
    CSResolveType type,
    CSDefaultValue? defaultValue
) : BaseCSAstItem
{
    public string Name => name;
    public CSResolveType Type => type;
    public CSDefaultValue? DefaultValue => defaultValue;
    public bool IsConst { get; init; }
    public bool IsStatic { get; init; }
    public bool IsReadOnly { get; init; }
    public CSAccessModifier AccessModifier { get; init; } = CSAccessModifier.Public;
}