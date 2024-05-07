using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSEnumTranslator : BaseTranslator
{
    public override void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var structItem in compilationUnit.GetEnumEnumerable())
            {
                if (translationUnit.IsTypeTranslated(structItem))
                {
                    continue;
                }

                outputChannel.OnReceiveEnum(TranslateEnum(structItem));
            }
        }
    }

    public override void SecondPass(
        CSTranslationUnit translationUnit,
        BaseTranslatorInputChannel inputChannel)
    {
        foreach (var enumItem in inputChannel.GetEnums())
        {
            enumItem.OnSecondPass(translationUnit);
        }
    }

    private static CSEnum TranslateEnum(CEnum enumItem)
    {
        List<CSEnumField> enumValue = [];

        foreach (var value in enumItem.Fields)
        {
            enumValue.Add(TranslateEnumField(value));
        }

        var newCSEnum = new CSEnum()
        {
            Name = enumItem.Name,
            Type = CSPrimitiveType.Instances.Int
        };
        newCSEnum.Values.AddRange(enumValue);
        newCSEnum.EnrichingDataStore.Add(new CSTranslationFromCAstData(enumItem));
        enumItem.EnrichingDataStore.Add(new CTranslationToCSAstData(newCSEnum));
        return newCSEnum;
    }

    private static CSEnumField TranslateEnumField(CEnumField enumField)
    {
        var cExpression = enumField.Expression ?? throw new InvalidOperationException("Enum field expression is null");
        var csExpression = CSConstantExpression.FromCConstantExpression(cExpression);

        CSEnumField newCSEnumValue = new()
        {
            Name = enumField.Name,
            Expression = csExpression
        };
        
        newCSEnumValue.EnrichingDataStore.Add(new CSTranslationFromCAstData(enumField));
        enumField.EnrichingDataStore.Add(new CTranslationToCSAstData(newCSEnumValue));
        return newCSEnumValue;
    }

}