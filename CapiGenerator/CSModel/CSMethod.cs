using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSMethod : BaseCSAstItem
{
    private readonly CSTypeInstance? _returnType;
    private string? _name;
    private string? _body;
    private CSAccessModifier _accessModifier = CSAccessModifier.Public;
    private bool _isStatic;
    private bool _isExtern;

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
        init
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
    public ChangeCountList<CSParameter> Parameters { get; init; } = [];

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        _returnType?.OnSecondPass(unit);
        foreach (var parameter in Parameters)
        {
            parameter.OnSecondPass(unit);
        }
    }
}