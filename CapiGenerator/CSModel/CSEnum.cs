using CapiGenerator.CModel;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSEnum : CSBaseType
{
    private readonly ResoleRef<ICSType, ICType> _rRefType;
    private readonly CSEnumValue[] _values;

    public CSEnum(string name, ICType type, ReadOnlySpan<CSEnumValue> values)
        : base(name)
    {
        _values = values.ToArray();
        _rRefType = new(type);
    }

    public CSEnum(string name, ICSType type, ReadOnlySpan<CSEnumValue> values)
        : base(name)
    {
        _values = values.ToArray();
        _rRefType = new(type);
    }


    public ResoleRef<ICSType, ICType> RRefType => _rRefType;
    public ICSType? Type => _rRefType.Output;
    public ReadOnlySpan<CSEnumValue> Values => _values;

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        _rRefType.TrySetOutputFromResolver(unit);
    }
}