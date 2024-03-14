using CapiGenerator.CModel;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSEnum : CSBaseType
{
    private readonly ResoleRef<ICSType, ICType> _rRefType;
    private readonly CSEnumField[] _values;
    private string? _fullName;

    public CSEnum(string name, ICType type, ReadOnlySpan<CSEnumField> values)
        : base(name)
    {
        _values = values.ToArray();
        _rRefType = new(type);
    }

    public CSEnum(string name, ICSType type, ReadOnlySpan<CSEnumField> values)
        : base(name)
    {
        _values = values.ToArray();
        _rRefType = new(type);
    }


    public ResoleRef<ICSType, ICType> RRefType => _rRefType;
    public ICSType? Type => _rRefType.Output;
    public ReadOnlySpan<CSEnumField> Values => _values;

    public string? Namespace { get; init; }
    public string FullName => _fullName ??= Namespace is null ? Name : $"{Namespace}.{Name}";

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        _rRefType.TrySetOutputFromResolver(unit);
    }
}