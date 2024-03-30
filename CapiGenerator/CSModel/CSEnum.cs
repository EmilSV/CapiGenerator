using CapiGenerator.CModel;
using CapiGenerator.Collection;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSEnum : CSBaseType
{
    private readonly ResoleRef<ICSType, ICType> _rRefType;
    private readonly HistoricList<CSEnumField> _values;
    private string? _fullName;

    public CSEnum(string name, ICType type, ReadOnlySpan<CSEnumField> values)
        : base(name)
    {
        _values = new(values);
        _rRefType = new(type);
        foreach (var value in _values)
        {
            value.SetParent(this);
        }
    }

    public CSEnum(string name, ICSType type, ReadOnlySpan<CSEnumField> values)
        : base(name)
    {
        _values = new(values);
        _rRefType = new(type);
        foreach (var value in _values)
        {
            value.SetParent(this);
        }
    }


    public ResoleRef<ICSType, ICType> RRefType => _rRefType;
    public ICSType? Type => _rRefType.Output;
    public HistoricList<CSEnumField> Values => _values;

    public string? Namespace { get; init; }
    public override string FullName => _fullName ??= Namespace is null ? Name : $"{Namespace}.{Name}";

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        _rRefType.TrySetOutputFromResolver(unit);
    }
}