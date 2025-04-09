using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSStruct : BaseCSType, ITypeReplace,
    INotifyReviver<CSField>,
    INotifyReviver<CSMethod>,
    INotifyReviver<CSConstructor>
{
    public CSStruct()
    {
        Fields = new(this);
        Methods = new(this);
        Interfaces = new(null);
        Constructors = new(this);
    }

    public CSAccessModifier AccessModifier;
    public bool IsUnsafe;
    public bool IsPartial;
    public bool IsReadOnly;

    public NotifySet<LazyFormatString> Interfaces { get; private set; }
    public NotifySet<CSField> Fields { get; private set; }
    public NotifySet<CSMethod> Methods { get; private set; }
    public NotifySet<CSConstructor> Constructors { get; private set; }
    public NotifyList<BaseCSAttribute> Attributes { get; } = new(null);

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

    public void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        foreach (var field in Fields)
        {
            field.ReplaceTypes(predicate);
        }
        foreach (var method in Methods)
        {
            method.ReplaceTypes(predicate);
        }
    }

    void INotifyReviver<CSField>.OnAddRange(ReadOnlySpan<CSField> items)
    {
        foreach (var item in items)
        {
            item.SetParent(this);
        }
    }

    void INotifyReviver<CSMethod>.OnAddRange(ReadOnlySpan<CSMethod> items)
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

    void INotifyReviver<CSMethod>.OnRemoveRange(ReadOnlySpan<CSMethod> items)
    {
        foreach (var item in items)
        {
            item.SetParent(null);
        }
    }

    void INotifyReviver<CSConstructor>.OnAddRange(ReadOnlySpan<CSConstructor> items)
    {
        foreach (var item in items)
        {
            item.SetParent(this);
        }
    }

    void INotifyReviver<CSConstructor>.OnRemoveRange(ReadOnlySpan<CSConstructor> items)
    {
        foreach (var item in items)
        {
            item.SetParent(null);
        }
    }
}

