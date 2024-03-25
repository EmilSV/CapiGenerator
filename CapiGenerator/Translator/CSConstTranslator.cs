using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;


public class CSConstTranslator(string className) : BaseTranslator
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
                if (!PredicateSelector(constant))
                {
                    break;
                }

                constantFields.Add(TranslateConstant(constant));
            }
        }

        var csStaticClass = new CSStaticClass(className, CollectionsMarshal.AsSpan(constantFields), []);

        outputChannel.OnReceiveStaticClass(csStaticClass);
    }

    public override void SecondPass(
        CSTranslationUnit translationUnit,
        BaseTranslatorInputChannel inputChannel)
    {
        foreach (var staticClass in inputChannel.GetStaticClasses())
        {
            staticClass.OnSecondPass(translationUnit);
        }
    }

    private CSField TranslateConstant(CConstant constant)
    {
        var cType = constant.Expression.GetTypeOfExpression();
        CSPrimitiveType csType = cType switch
        {
            CConstantType.Char => CSPrimitiveType.Get(CSPrimitiveType.Kind.Byte),
            CConstantType.Int => CSPrimitiveType.Get(CSPrimitiveType.Kind.Int),
            CConstantType.Float => CSPrimitiveType.Get(CSPrimitiveType.Kind.Float),
            CConstantType.String => CSPrimitiveType.Get(CSPrimitiveType.Kind.String),
            _ => throw new Exception("Unknown constant type"),
        };

        var typeInstance = new CSTypeInstance(csType);
        var csConstantExpression = CSConstantExpression.FromCConstantExpression(constant.Expression);
        var newCSField = new CSField(NameSelector(constant), typeInstance, new(csConstantExpression));
        newCSField.EnrichingDataStore.Add(new CSTranslationFromCAstData(constant));
        return newCSField;
    }


}