using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSStructTranslator : BaseTranslator
{
    public override void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var structItem in compilationUnit.GetStructEnumerable())
            {
                if (translationUnit.IsTypeTranslated(structItem))
                {
                    continue;
                }

                outputChannel.OnReceiveStruct(TranslateStruct(structItem));
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

    protected static CSStruct TranslateStruct(CStruct structItem)
    {
        List<CSField> fields = [];

        foreach (var field in structItem.Fields)
        {
            fields.Add(TranslateField(field));
        }

        var newCSStruct = new CSStruct
        {
            Name = structItem.Name,
        };
        newCSStruct.Fields.AddRange(fields);

        newCSStruct.EnrichingDataStore.Set(new CSTranslationFromCAstData(structItem));
        structItem.EnrichingDataStore.Set(new CTranslationToCSAstData(newCSStruct));
        return newCSStruct;
    }

    protected static BaseCSTypeModifier[] TranslateModifiers(ReadOnlySpan<CTypeModifier> cModifiers)
    {
        List<BaseCSTypeModifier> modifiers = [];

        foreach (var cModifier in cModifiers)
        {
            BaseCSTypeModifier modifier = cModifier switch
            {
                PointerType => CsPointerType.Instance,
                _ => throw new Exception("unsupported modifier"),
            };
            modifiers.Add(modifier);
        }


        return [.. modifiers];
    }

    protected static CSField TranslateField(CField field)
    {
        var cTypeInstance = field.GetFieldType();
        var csTypeInstance = CSTypeInstance.CreateFromCTypeInstance(cTypeInstance);
        var newField = new CSField
        {
            Name = field.Name,
            Type = csTypeInstance,
        };

        newField.EnrichingDataStore.Set(new CSTranslationFromCAstData(field));
        field.EnrichingDataStore.Set(new CTranslationToCSAstData(newField));

        return newField;
    }


}