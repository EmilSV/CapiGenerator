using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;


public class CConstTranslator(string className) : BaseTranslator
{
    protected virtual string NameSelector(CConstant value) => value.Name;
    protected virtual bool PredicateSelector(CConstant value) => true;


    public override void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        List<CSField> constantFields = [];
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var constant in compilationUnit.GetConstantEnumerable())
            {
                if (PredicateSelector(constant))
                {
                    constantFields.Add(TranslateConstant(constant));
                }
            }
        }
    }

    private CSField TranslateConstant(CConstant constant)
    {
        var cType = constant.ConstantExpression.GetTypeOfExpression();
        CSPrimitiveType csType = cType switch
        {
            CConstantType.Char => CSPrimitiveType.Get(CSPrimitiveType.Kind.Char),
            CConstantType.Int => CSPrimitiveType.Get(CSPrimitiveType.Kind.Int),
            CConstantType.Float => CSPrimitiveType.Get(CSPrimitiveType.Kind.Float),
            CConstantType.String => CSPrimitiveType.Get(CSPrimitiveType.Kind.String),
            _ => throw new Exception("Unknown constant type"),
        };

        var typeInstance = new CSTypeInstance(csType);
        var newCSField = new CSField(NameSelector(constant), typeInstance, );
        newCSField.EnrichingDataStore.Add(new CSTranslationFromCAstData(constant));
        constant.EnrichingDataStore.Add(new CSTranslationsTypeData(newCSField));
        return newCSField;
    }


}