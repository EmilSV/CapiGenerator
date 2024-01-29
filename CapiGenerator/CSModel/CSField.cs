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
        public Options()
        {
        }

        public bool Const { get; init; }
        public bool isStatic { get; init; }
        public bool isReadOnly { get; init; }
        public bool isPrivate { get; init; }
        public bool isProtected { get; init; }
        public bool isPublic { get; init; } = true;
    }

    public string Name => name;
    public CSResolveType Type => type;
    public CSDefaultValue? DefaultValue => defaultValue;
    public bool IsConst => options.Const;
}