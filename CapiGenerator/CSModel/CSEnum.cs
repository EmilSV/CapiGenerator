using CapiGenerator.CModel;
using CapiGenerator.UtilTypes;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSEnum : CSBaseType
{
    private ResoleRef<ICSType, ICType> _rRefType;

    public ResoleRef<ICSType, ICType> RRefType
    {
        get => _rRefType;
        init => _rRefType = value;
    }
    public ICSType? Type
    {
        get => _rRefType.Output;
        set
        {
            if (_rRefType.Output != value)
            {
                _rRefType = new(value);
                NotifyChange();
            }
        }
    }

    public ChangeCountList<CSEnumField> Values { get; init; } = [];

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        _rRefType.TrySetOutputFromResolver(unit);
    }
}