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

        CConstantType constantType = CConstantType.NONE;
        foreach (var value in enumItem.Fields)
        {
            var cExpression = value.Expression ?? throw new InvalidOperationException("Enum field expression is null");
            var expressionType = cExpression.GetTypeOfExpression();
            constantType = expressionType switch
            {
                CConstantType.Int
                    when constantType is CConstantType.NONE or CConstantType.Int => CConstantType.Int,
                CConstantType.Float or CConstantType.Int
                    when constantType is CConstantType.NONE or CConstantType.Float or CConstantType.Int => CConstantType.Float,
                CConstantType.Char
                    when constantType is CConstantType.NONE or CConstantType.Char => CConstantType.Char,
                CConstantType.String or CConstantType.Char
                    when constantType is CConstantType.NONE or CConstantType.String or CConstantType.Char => CConstantType.String,
                _ => CConstantType.Unknown
            };
        }

        if (constantType is CConstantType.Unknown or CConstantType.NONE)
        {
            throw new InvalidOperationException("Enum field expression type is unknown");
        }

        CSPrimitiveType csType = constantType switch
        {
            CConstantType.Int => CSPrimitiveType.Get(CSPrimitiveType.Kind.Int),
            CConstantType.Float => CSPrimitiveType.Get(CSPrimitiveType.Kind.Float),
            CConstantType.String => CSPrimitiveType.Get(CSPrimitiveType.Kind.String),
            CConstantType.Char => CSPrimitiveType.Get(CSPrimitiveType.Kind.Byte),
            _ => throw new InvalidOperationException("Enum field expression type is unknown")
        };

        var newCSEnum = new CSEnum()
        {
            Name = enumItem.Name,
            RRefType = new(csType),
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