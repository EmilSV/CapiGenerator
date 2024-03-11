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
                outputChannel.OnReceiveStruct(TranslateStruct(structItem, translationUnit));
            }
        }
    }

    protected static CSStruct TranslateStruct(CStruct structItem, CSTranslationUnit translationUnit)
    {
        List<CSField> fields = [];

        foreach (var field in structItem.Fields)
        {
            fields.Add(TranslateField(field));
        }

        var newCSStruct = new CSStruct(structItem.Name, fields.ToArray(), []);
        newCSStruct.EnrichingDataStore.Add(new CSTranslationFromCAstData(structItem));
        structItem.EnrichingDataStore.Add(new CTranslationToCSAstData(newCSStruct));
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
        var typeModifiers = TranslateModifiers(cTypeInstance.Modifiers);
        var cType = cTypeInstance.GetCType() ?? throw new Exception("cType is null");

        var csTypeInstance = new CSTypeInstance(cType, typeModifiers);
        var newField = new CSField(field.Name, csTypeInstance);

        newField.EnrichingDataStore.Add(new CSTranslationFromCAstData(field));
        field.EnrichingDataStore.Add(new CTranslationToCSAstData(newField));

        return newField;
    }


}