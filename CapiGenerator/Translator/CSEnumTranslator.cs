using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.Comments;
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

                outputChannel.OnReceiveEnum(TranslateEnum(structItem, compilationUnit));
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

    private static CSEnum TranslateEnum(CEnum enumItem, CCompilationUnit compilationUnit)
    {
        List<CSEnumField> enumValue = [];

        foreach (var value in enumItem.Fields)
        {
            enumValue.Add(TranslateEnumField(value));
        }

        var newCSEnum = new CSEnum(enumItem)
        {
            Name = enumItem.Name,
            Type = CSPrimitiveType.Instances.Int,
            Comments = CCommentTranslator.Translate(enumItem.Comment, compilationUnit),
        };
        newCSEnum.Values.AddRange(enumValue);
        enumItem.AddDerivative(newCSEnum);
        return newCSEnum;
    }

    private static CSEnumField TranslateEnumField(CEnumField enumField)
    {
        var cExpression = enumField.Expression ?? throw new InvalidOperationException("Enum field expression is null");
        var csExpression = CSConstantExpression.FromCConstantExpression(cExpression);

        CSEnumField newCSEnumValue = new(enumField)
        {
            Name = enumField.Name,
            Expression = csExpression
        };

        enumField.AddDerivative(newCSEnumValue);
        return newCSEnumValue;
    }

}
