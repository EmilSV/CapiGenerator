using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSStaticClass : BaseCSType, INotifyReviver<CSField>, INotifyReviver<CSMethod>
{
    public NotifySet<CSField> Fields { get; private set; }
    public NotifySet<CSMethod> Methods { get; private set; }

    public CSStaticClass()
    {
        Fields = new(this);
        Methods = new(this);
    }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var field in Fields)
        {
            field.OnSecondPass(unit);
        }
        foreach (var method in Methods)
        {
            method.OnSecondPass(unit);
        }
    }

    void INotifyReviver<CSField>.OnAddRange(ReadOnlySpan<CSField> items)
    {
        foreach (var item in items)
        {
            item.SetParent(this);
        }
    }

    void INotifyReviver<CSField>.OnRemoveRange(ReadOnlySpan<CSField> items)
    {
        foreach (var item in items)
        {
            item.SetParent(null);
        }
    }

    void INotifyReviver<CSMethod>.OnAddRange(ReadOnlySpan<CSMethod> items)
    {
        foreach (var item in items)
        {
            item.SetParent(this);
        }
    }

    void INotifyReviver<CSMethod>.OnRemoveRange(ReadOnlySpan<CSMethod> items)
    {
        foreach (var item in items)
        {
            item.SetParent(null);
        }
    }
}

