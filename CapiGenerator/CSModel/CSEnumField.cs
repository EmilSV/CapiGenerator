using CapiGenerator.UtilTypes;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSEnumField(string name, CSConstantExpression expression) : BaseCSAstItem
{
    private CSBaseType? _parent;

    public CSBaseType? Parent => _parent;

    private readonly HistoricValue<string> _name = new(name);
    public HistoricValue<string> Name => _name;

    private readonly HistoricValue<CSConstantExpression> _expression = new(expression);
    public HistoricValue<CSConstantExpression> Expression => _expression;

    public void SetParent(CSBaseType parent)
    {
        if (_parent is not null)
        {
            throw new InvalidOperationException("Parent is already set");
        }

        _parent = parent;
    }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var expression in _expression.GetHistoricValues())
        {
            expression.OnSecondPass(unit);
        }
    }
}