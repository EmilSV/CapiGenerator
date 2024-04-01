using CapiGenerator.CModel;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSField : BaseCSAstItem, ICSField
{
    private CSBaseType? _parent;

    public CSField(string name, CSTypeInstance type, CSDefaultValue defaultValue = default)
    {
        Name = new(name);
        Type = new(type);
        DefaultValue = new(defaultValue);
        FullName = new(
            dependencies: [Name],
            compute: () => Parent is not null ? $"{Parent.FullName.Value}.{Name.Value}" : Name.Value!
        );
    }

    public CSBaseType? Parent => _parent;
    public HistoricValue<string> Name { get; }
    public ComputedValue<string> FullName { get; }
    public HistoricValue<CSTypeInstance> Type { get; }
    public HistoricValue<CSDefaultValue> DefaultValue { get; }

    public HistoricValue<bool> IsConst { get; } = new(false);
    public HistoricValue<bool> IsStatic { get; } = new(false);
    public HistoricValue<bool> IsReadOnly { get; } = new(false);
    public HistoricValue<CSAccessModifier> AccessModifier { get; } = new(CSAccessModifier.Public);

    string ICSField.Name => Name!;
    string ICSField.FullName => FullName;

    internal void SetParent(CSBaseType parent)
    {
        if (_parent is not null)
        {
            throw new InvalidOperationException("Parent is already set");
        }
        _parent = parent;
        if (parent.FullName.TryAsComputedValue(out var parentFullName))
        {
            FullName.AddDependency(parentFullName);
        }
    }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        Type.Value?.OnSecondPass(unit);
    }
}