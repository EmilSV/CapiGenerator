using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSMethod : BaseCSAstItem,
    INotifyReviver<CSParameter>, ITypeReplace
{
    private CSTypeInstance? _returnType;
    private string? _name;
    private LazyFormatString? _body;
    private CSAccessModifier _accessModifier = CSAccessModifier.Public;
    private bool _isStatic;
    private bool _isExtern;
    private bool _isOverride;
    private bool _isVirtual;
    private bool _isReadonly;
    private bool _isNew;
    private bool _isAbstract;
    private bool _isAsync;
    private bool _isPartial;
    private CSMethodOperatorModifier _operatorModifier = CSMethodOperatorModifier.None;

    public BaseCSType? ParentType { get; private set; }


    public CSMethod()
    {
        Parameters = new(this);
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        CSTypeInstance returnType,
        string name,
        ReadOnlySpan<CSParameter> parameters
    )
    {
        ReturnType = returnType;
        _name = name;
        Parameters = new(this, parameters);
        _accessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        _isExtern = (modifiers & CSClassMemberModifier.Extern) != 0;
        _isOverride = (modifiers & CSClassMemberModifier.Override) != 0;
        _isStatic = (modifiers & CSClassMemberModifier.Static) != 0;
        _isVirtual = (modifiers & CSClassMemberModifier.Virtual) != 0;
        _isReadonly = (modifiers & CSClassMemberModifier.ReadOnly) != 0;
        _isNew = (modifiers & CSClassMemberModifier.New) != 0;
        _isAbstract = (modifiers & CSClassMemberModifier.Abstract) != 0;
        _isAsync = (modifiers & CSClassMemberModifier.Async) != 0;
        _isPartial = (modifiers & CSClassMemberModifier.Partial) != 0;
        _operatorModifier = CSMethodOperatorModifierHelper.GetOperatorModifier(modifiers);
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        ICSType returnType,
        string name,
        ReadOnlySpan<CSParameter> parameters
    ) : this(modifiers, new CSTypeInstance(returnType), name, parameters)
    {
    }


    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        CSTypeInstance returnType,
        string name,
        ReadOnlySpan<(CSTypeInstance type, string name)> parameters
    ) : this(modifiers, returnType, name, GetParameters(parameters))
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        ICSType returnType,
        string name,
        ReadOnlySpan<(CSTypeInstance type, string name)> parameters
    ) : this(modifiers, returnType, name, GetParameters(parameters))
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        CSTypeInstance returnType,
        string name,
        ReadOnlySpan<(ICSType type, string name)> parameters
    ) : this(modifiers, returnType, name, GetParameters(parameters))
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        ICSType returnType,
        string name,
        ReadOnlySpan<(ICSType type, string name)> parameters
    ) : this(modifiers, new CSTypeInstance(returnType), name, GetParameters(parameters))
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        CSTypeInstance returnType,
        ReadOnlySpan<(ICSType type, string name)> parameters
    ) : this(modifiers, returnType, "", GetParameters(parameters))
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
    CSClassMemberModifier modifiers,
        CSTypeInstance returnType,
        ReadOnlySpan<(CSTypeInstance type, string name)> parameters
    ) : this(modifiers, returnType, "", GetParameters(parameters))
    {
    }


    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        ICSType returnType,
        ReadOnlySpan<(ICSType type, string name)> parameters
    ) : this(modifiers, returnType, "", GetParameters(parameters))
    {
    }


    public string? Name
    {
        get => _name;
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
    public LazyFormatString? Body
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

    public bool IsOverride
    {
        get => _isOverride;
        set
        {
            if (_isOverride != value)
            {
                _isOverride = value;
                NotifyChange();
            }
        }
    }

    public bool IsVirtual
    {
        get => _isVirtual;
        set
        {
            if (_isVirtual != value)
            {
                _isVirtual = value;
                NotifyChange();
            }
        }
    }

    public bool IsReadonly
    {
        get => _isReadonly;
        set
        {
            if (_isReadonly != value)
            {
                _isReadonly = value;
                NotifyChange();
            }
        }
    }

    public bool IsNew
    {
        get => _isNew;
        set
        {
            if (_isNew != value)
            {
                _isNew = value;
                NotifyChange();
            }
        }
    }

    public bool IsAbstract
    {
        get => _isAbstract;
        set
        {
            if (_isAbstract != value)
            {
                _isAbstract = value;
                NotifyChange();
            }
        }
    }

    public bool IsAsync
    {
        get => _isAsync;
        set
        {
            if (_isAsync != value)
            {
                _isAsync = value;
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

    public CSMethodOperatorModifier OperatorModifier
    {
        get => _operatorModifier;
        set
        {
            if (_operatorModifier != value)
            {
                _operatorModifier = value;
                NotifyChange();
            }
        }
    }


    public NotifyList<CSParameter> Parameters { get; }
    public NotifyList<BaseCSAttribute> Attributes { get; } = new(null);

    public CommentSummery? Comments { get; set; }

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
                ReturnType = CSTypeInstance.CopyWithNewType(ReturnType, newType!);
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
                Parameters.TryReplaceAt(i, CSParameter.CopyWithNewType(parameter, newType!));
            }
        }
    }

    private static CSParameter[] GetParameters(ReadOnlySpan<(CSTypeInstance type, string name)> parameters)
    {
        if (parameters.Length == 0)
        {
            return Array.Empty<CSParameter>();
        }

        var array = new CSParameter[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var (type, name) = parameters[i];
            array[i] = new(type, name);
        }
        return array;
    }

    private static CSParameter[] GetParameters(ReadOnlySpan<(ICSType type, string name)> parameters)
    {
        if (parameters.Length == 0)
        {
            return Array.Empty<CSParameter>();
        }

        var array = new CSParameter[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var (type, name) = parameters[i];
            array[i] = new(new(type), name);
        }
        return array;
    }
}