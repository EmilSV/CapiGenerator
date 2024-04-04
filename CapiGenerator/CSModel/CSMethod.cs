using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSMethod : BaseCSAstItem
{
    private readonly HistoricValue<CSTypeInstance> _returnType;
    private readonly HistoricValue<string> _name;
    private readonly HistoricList<CSParameter> _parameters;
    private readonly HistoricValue<string?> _body;
    private CSBaseType? _parent;

    public CSMethod(
        CSTypeInstance returnType, string name,
        ReadOnlySpan<CSParameter> parameters, string? body = null)
    {
        _returnType = new(returnType);
        _name = new(name);
        _parameters = new(parameters);
        _body = new(body);
        FullName = new ComputedValue<string>(
            dependencies: new[] { _name },
            compute: () => _parent != null ? $"{_parent.FullName.Value}.{_name.Value}" : _name.Value!
        );
    }

    public HistoricValue<CSTypeInstance> ReturnType => _returnType;
    public HistoricValue<string> Name => _name;
    public HistoricList<CSParameter> Parameters => _parameters;
    public HistoricValue<string?> Body => _body;

    public HistoricValue<bool> IsStatic { get; } = new(false);
    public HistoricValue<bool> IsExtern { get; } = new(false);

    public HistoricValue<CSAccessModifier> AccessModifier { get; } = new(CSAccessModifier.Public);
    public ComputedValue<string> FullName { get; }


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
        _returnType.Value?.OnSecondPass(unit);
        foreach (var parameter in _parameters)
        {
            parameter.OnSecondPass(unit);
        }
    }
}