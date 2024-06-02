using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSMethod : BaseCSAstItem,
    INotifyReviver<CSParameter>, ITypeReplace
{
    private CSTypeInstance? _returnType;
    private string? _name;
    private string? _body;
    private CSAccessModifier _accessModifier = CSAccessModifier.Public;
    private bool _isStatic;
    private bool _isExtern;
    public BaseCSType? ParentType { get; private set; }


    public CSMethod()
    {
        Parameters = new(this);
    }

    public required string Name
    {
        get => _name!;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Name cannot be null or empty");
            }
            if (_name != value)
            {
                _name = value;
                NotifyChange();
            }
        }
    }
    public string? Body
    {
        get => _body;
        set
        {
            if (_body != value)
            {
                _body = value;
                NotifyChange();
            }
        }
    }

    public required CSTypeInstance ReturnType
    {
        get => _returnType!;
        set
        {
            if (_returnType != value)
            {
                _returnType = value;
                NotifyChange();
            }
        }
    }
    public bool IsStatic
    {
        get => _isStatic;
        set
        {
            if (_isStatic != value)
            {
                _isStatic = value;
                NotifyChange();
            }
        }
    }
    public bool IsExtern
    {
        get => _isExtern;
        set
        {
            if (_isExtern != value)
            {
                _isExtern = value;
                NotifyChange();
            }
        }
    }
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

    public NotifyUniqueList<CSParameter> Parameters { get; }

    public NotifyList<BaseCSAttribute> Attributes { get; } = new(null);

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        _returnType?.OnSecondPass(unit);
        foreach (var parameter in Parameters)
        {
            parameter.OnSecondPass(unit);
        }
    }

    internal void SetParent(BaseCSType? parent)
    {
        if (ParentType != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }
        ParentType = parent;
        NotifyChange();
    }

    void INotifyReviver<CSParameter>.OnAddRange(ReadOnlySpan<CSParameter> items)
    {
        foreach (var item in items)
        {
            item.SetParentMethod(this);
        }
    }

    void INotifyReviver<CSParameter>.OnRemoveRange(ReadOnlySpan<CSParameter> items)
    {
        foreach (var item in items)
        {
            item.SetParentMethod(null);
        }
    }

    public void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        var innerReturnType = ReturnType.Type;

        if (innerReturnType is not null)
        {
            if (predicate(innerReturnType, out var newType))
            {
                ReturnType = CSTypeInstance.CopyWithNewType(ReturnType, newType);
            }
        }
        else
        {
            Console.Error.WriteLine($"method {Name} has null return type and cannot be replaced");
        }

        int count = Parameters.Count;
        for (int i = 0; i < count; i++)
        {
            var parameter = Parameters[i];
            var innerType = parameter.Type.Type;
            if (innerType is null)
            {
                Console.Error.WriteLine($"method {Name} has null parameter type and cannot be replaced");
                continue;
            }
            if (predicate(innerType, out var newType))
            {
                Parameters.TryReplaceAt(i, CSParameter.CopyWithNewType(parameter, newType));
            }
        }
    }
}