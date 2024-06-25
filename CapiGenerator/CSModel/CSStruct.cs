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

    private bool _isPartial;
    private bool _isReadOnly;
    private bool _isUnsafe;
    private CSAccessModifier _accessModifier;

    public CSAccessModifier AccessModifier
    {
        get => _accessModifier;
        set
        {
            if (_accessModifier != value)
            {
                _accessModifier = value;
                NotifyChange();
            }
        }
    }


    public bool IsUnsafe
    {
        get => _isUnsafe;
        set
        {
            if (_isUnsafe != value)
            {
                _isUnsafe = value;
                NotifyChange();
            }
        }
    }

    public bool IsPartial
    {
        get => _isPartial;
        set
        {
            if (_isPartial != value)
            {
                _isPartial = value;
                NotifyChange();
            }
        }
    }

    public bool IsReadOnly
    {
        get => _isReadOnly;
        set
        {
            if (_isReadOnly != value)
            {
                _isReadOnly = value;
                NotifyChange();
            }
        }
    }

    public NotifySet<LazyFormatString> Interfaces { get; private set; }
    public NotifySet<CSField> Fields { get; private set; }
    public NotifySet<CSMethod> Methods { get; private set; }
    public NotifySet<CSConstructor> Constructors { get; private set; }

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

