using CapiGenerator.CModel;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSField(
    string name, CSTypeInstance type, CSDefaultValue defaultValue = default
) : BaseCSAstItem, ICSField
{
    private CSBaseType? _parent;

    public CSBaseType? Parent => _parent;
    public string Name => name;
    public string FullName => Parent is not null ? $"{Parent.FullName}.{Name}" : Name;
    public CSTypeInstance Type => type;
    public CSDefaultValue DefaultValue => defaultValue;

    public bool IsConst { get; init; }
    public bool IsStatic { get; init; }
    public bool IsReadOnly { get; init; }
    public CSAccessModifier AccessModifier { get; init; } = CSAccessModifier.Public;

    public void SetParent(CSBaseType parent)
    {
        if (_parent is not null)
        {
            throw new InvalidOperationException("Parent is already set");
        }

        _parent = parent;
    }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        type.OnSecondPass(unit);
    }
}