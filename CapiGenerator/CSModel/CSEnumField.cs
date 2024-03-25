using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSEnumField(string name, CSConstantExpression expression) : BaseCSAstItem
{
    private CSBaseType? _parent;

    public CSBaseType? Parent => _parent;

    public string Name => name;
    public CSConstantExpression Expression => expression;

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
        expression.OnSecondPass(unit);
    }
}