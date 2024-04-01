using CapiGenerator.CModel;
using CapiGenerator.UtilTypes;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSEnum : CSBaseType
{
    private readonly HistoricResoleRef<ICSType, ICType> _rRefType;
    private readonly HistoricList<CSEnumField> _values;
    private ComputedValue<string> _fullName;

    private CSEnum(string name, HistoricResoleRef<ICSType, ICType> refType, ReadOnlySpan<CSEnumField> values)
        : base(name)
    {
        _values = new(values);
        _rRefType = refType;
        foreach (var value in _values)
        {
            value.SetParent(this);
        }

        _fullName = new ComputedValue<string>(
          dependencies: [Namespace, Name],
          compute: () => Namespace != null ? $"{Namespace.Value}.{Name.Value}" : Name.Value
        );
    }

    public CSEnum(string name, ICType type, ReadOnlySpan<CSEnumField> values)
        : this(name, new HistoricResoleRef<ICSType, ICType>(type), values)
    {
    }

    public CSEnum(string name, ICSType type, ReadOnlySpan<CSEnumField> values)
        : this(name, new HistoricResoleRef<ICSType, ICType>(type), values)
    {
    }

    public HistoricResoleRef<ICSType, ICType> RRefType => _rRefType;
    public ICSType? Type => _rRefType.Output;
    public HistoricList<CSEnumField> Values => _values;

    public HistoricValue<string?> Namespace { get; } = new(null);
    public override ComputedValueOrValue<string> FullName => _fullName;

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        _rRefType.TrySetOutputFromResolver(unit);
    }
}