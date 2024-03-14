using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSEnumField(string name, CSConstantExpression expression) : BaseCSAstItem
{
    public string Name => name;
    public CSConstantExpression Expression => expression;

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        expression.OnSecondPass(unit);
    }
}