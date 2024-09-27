using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSField : BaseCSAstItem,
    ICSFieldLike, ITypeReplace
{
    private string? _name;
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

    private CSTypeInstance? _type;
    public required CSTypeInstance Type
    {
        get => _type!;
        set
        {
            if (_type != value)
            {
                _type = value;
                NotifyChange();
            }
        }
    }

    private CSDefaultValue _defaultValue;
    public CSDefaultValue DefaultValue
    {
        get => _defaultValue;
        set
        {
            if (_defaultValue != value)
            {
                _defaultValue = value;
                NotifyChange();
            }
        }
    }

    private bool _isConst;
    public bool IsConst
    {
        get => _isConst;
        set
        {
            if (_isConst != value)
            {
                _isConst = value;
                NotifyChange();
            }
        }
    }

    private bool _isStatic;
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

    private bool _isReadOnly;
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

    private bool _isRequired;
    public bool IsRequired
    {
        get => _isRequired;
        set
        {
            if (_isRequired != value)
            {
                _isRequired = value;
                NotifyChange();
            }
        }
    }

    public CommentSummery? Comments { get; set; }

    private CSAccessModifier _accessModifier = CSAccessModifier.Public;
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

    private CSPropertyBody? _getterBody;
    public CSPropertyBody? GetterBody
    {
        get => _getterBody;
        set
        {
            if (_getterBody != value)
            {
                _getterBody = value;
                NotifyChange();
            }
        }
    }

    private CSPropertyBody? _setterBody;
    public CSPropertyBody? SetterBody
    {
        get => _setterBody;
        set
        {
            if (_setterBody != value)
            {
                _setterBody = value;
                NotifyChange();
            }
        }
    }

    public BaseCSType? ParentType { get; private set; }


    public CSField()
    {

    }

#pragma warning disable 8618
    [SetsRequiredMembers]
    public CSField(CSClassMemberModifier modifier, CSTypeInstance type, string name)
    {
        _accessModifier = CSAccessModifierHelper.GetAccessModifier(modifier);
        _type = type;
        _name = name;
        _isConst = (modifier & CSClassMemberModifier.Const) != 0;
        _isReadOnly = (modifier & CSClassMemberModifier.ReadOnly) != 0;
        _isStatic = (modifier & CSClassMemberModifier.Static) != 0;
    }
#pragma warning restore 8618

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        Type.OnSecondPass(unit);
        DefaultValue.OnSecondPass(unit);
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

    public string GetFullName()
    {
        if (ParentType == null)
        {
            throw new InvalidOperationException("Parent type is not set");
        }

        return $"{ParentType.GetFullName()}.{Name}";
    }

    public void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        var innerType = Type.Type;
        if (innerType is null)
        {
            Console.Error.WriteLine($"field {Name} has null type and cannot be replaced");
            return;
        }

        if (innerType is BaseCSAnonymousType anonymousType)
        {
            anonymousType.ReplaceTypes(predicate);
            return;
        }

        if (predicate(innerType, out var newType))
        {
            Type = CSTypeInstance.CopyWithNewType(Type, newType!);
        }
    }
}