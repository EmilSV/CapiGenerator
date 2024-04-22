using CapiGenerator.CModel;
using CapiGenerator.UtilTypes;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSEnum : BaseCSType, INotifyReviver<CSEnumField>
{
    private ResoleRef<ICSType, ICType> _rRefType;

    public CSEnum()
    {
        Values = new(this);
    }

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

    public NotifyUniqueList<CSEnumField> Values { get; private set; }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        _rRefType.TrySetOutputFromResolver(unit);
    }

    void INotifyReviver<CSEnumField>.OnAddRange(ReadOnlySpan<CSEnumField> items)
    {
        foreach (var item in items)
        {
            item.SetParent(this);
        }
    }

    void INotifyReviver<CSEnumField>.OnRemoveRange(ReadOnlySpan<CSEnumField> items)
    {
        foreach (var item in items)
        {
            item.SetParent(null);
        }
    }
}