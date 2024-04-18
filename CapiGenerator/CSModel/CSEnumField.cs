using CapiGenerator.UtilTypes;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSEnumField : BaseCSAstItem, ICSFieldLike
{
    public CSEnum? ParentEnum { get; private set; }

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

    private CSConstantExpression _expression = [];
    public CSConstantExpression Expression
    {
        get => _expression;
        set
        {
            if (_expression != value)
            {
                _expression = value;
                NotifyChange();
            }
        }
    }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var expression in _expression)
        {
            expression.OnSecondPass(unit);
        }
    }

    internal void SetParent(CSEnum? parent)
    {
        if (ParentEnum != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }
        ParentEnum = parent;
        NotifyChange();
    }
}