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
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseCSTypeResolver typeResolver,
        BaseTranslatorOutputChannel outputChannel)
    {
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var structItem in compilationUnit.GetEnumEnumerable())
            {
                outputChannel.OnReceiveEnum(TranslateEnum(structItem));
            }
        }
    }

    private static CSEnum TranslateEnum(CEnum enumItem)
    {
        List<CSEnumValue> enumValue = [];

        foreach (var value in enumItem.GetValues())
        {
            enumValue.Add(TranslateEnumValue(value));
        }

        CSResolveType resolvedType = new(
            cType: CPrimitiveType.GetByKind(CPrimitiveType.Kind.Int),
            resolvedType: CSPrimitiveType.Get(CSPrimitiveType.Kind.Int)
        );

        var newCSEnum = new CSEnum(enumItem.Name, resolvedType, enumValue.ToArray());
        newCSEnum.EnrichingDataStore.Add(new CSTranslationFromCAstData(enumItem));
        enumItem.EnrichingDataStore.Add(new CSTranslationsTypeData(newCSEnum));
        return newCSEnum;
    }

    private static CSEnumValue TranslateEnumValue(CConstant constant)
    {
        CSEnumValue newCSEnumValue;
        if (constant.GetTokens() is [CConstLiteralToken literalToken] &&
            literalToken.TryParseValueAsInteger(out var value))
        {
            newCSEnumValue = new CSEnumValue(constant.Name, value);
        }
        else
        {
            newCSEnumValue = new CSEnumValue(constant.Name, constant);
        }

        newCSEnumValue.EnrichingDataStore.Add(new CSTranslationFromCAstData(constant));
        return newCSEnumValue;
    }
}