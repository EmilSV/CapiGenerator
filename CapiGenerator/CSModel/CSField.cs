using CapiGenerator.CModel;

namespace CapiGenerator.CSModel;

public class CSField(
    string name,
    CSResolveType type,
    CSDefaultValue? defaultValue,
    CSField.Options options = default
) : BaseCSAstItem
{
    public readonly record struct Options
    {
        public bool Const { get; init; }
    }

    public string Name => name;
    public CSResolveType Type => type;
    public CSDefaultValue? DefaultValue => defaultValue;
    public bool IsConst => options.Const;
}