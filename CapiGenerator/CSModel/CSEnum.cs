using CapiGenerator.CModel;
using CapiGenerator.UtilTypes;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSEnum : BaseCSType, INotifyReviver<CSEnumField>
{
    private CSPrimitiveType? _type;

    public CSEnum()
    {
        Values = new(this);
    }

    public CSPrimitiveType? Type
    {
        get => _type;
        set
        {
            if (_type != value)
            {
                _type = value;
                NotifyChange();
            }
        }
    }

    public NotifyUniqueList<CSEnumField> Values { get; private set; }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var value in Values)
        {
            value.OnSecondPass(unit);
        }
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