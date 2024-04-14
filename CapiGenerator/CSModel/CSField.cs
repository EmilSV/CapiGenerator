using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSField : BaseCSAstItem, ICSFieldLike
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
        init
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
        init
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

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        Type.OnSecondPass(unit);
    }
}