using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSTypedefTranslator : BaseTranslator
{
    public override void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var typedefItem in compilationUnit.GetTypedefEnumerable())
            {
                if (translationUnit.IsTypeTranslated(typedefItem))
                {
                    continue;
                }

                if (IsDirectTypedef(typedefItem))
                {
                    continue;
                }

                outputChannel.OnReceiveStruct(TranslateTypedef(typedefItem));
            }
        }
    }

    public override void SecondPass(
        CSTranslationUnit translationUnit,
        BaseTranslatorInputChannel inputChannel)
    {
        foreach (var structItem in inputChannel.GetStructs())
        {
            structItem.OnSecondPass(translationUnit);
        }
    }

    private static bool IsDirectTypedef(CTypedef typedef)
    {
        var innerCType = typedef.InnerType.GetCType();

        if (innerCType is CTypedef || innerCType is null)
        {
            return false;
        }

        if (typedef.InnerType.Modifiers.Length > 0)
        {
            return false;
        }

        return innerCType.Name == typedef.Name;
    }

    protected static CSStruct TranslateTypedef(CTypedef typedefItem)
    {
        var newCSStruct = new CSStruct
        {
            Name = typedefItem.Name,
        };
        newCSStruct.Fields.Add(new()
        {
            Name = "Value",
            Type = CSTypeInstance.CreateFromCTypeInstance(typedefItem.InnerType)
        });

        newCSStruct.EnrichingDataStore.Add(new CSTranslationFromCAstData(typedefItem));
        typedefItem.EnrichingDataStore.Add(new CTranslationToCSAstData(newCSStruct));
        return newCSStruct;
    }
}