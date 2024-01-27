using CapiGenerator.CModel;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSStructTranslator : BaseTranslator
{
    public override void Translator(
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var structItem in compilationUnit.GetStructEnumerable())
            {
                outputChannel.OnReceiveStruct(TranslateStruct(structItem));
            }
        }
    }

    private static CSStruct TranslateStruct(CStruct structItem)
    {
        List<CSField> fields = [];

        foreach (var field in structItem.Fields)
        {
            fields.Add(TranslateField(field));
        }

        var newCSStruct = new CSStruct(structItem.Name, fields.ToArray(), []);
        newCSStruct.EnrichingDataStore.Add(new CSTranslationCAstData(structItem));
        structItem.EnrichingDataStore.Add(new CSTranslationsTypeData(newCSStruct));
        return newCSStruct;
    }

    private static CSField TranslateField(CField field)
    {
        var newField = new CSField(field.Name, new(field.GetFieldType()), null);
        newField.EnrichingDataStore.Add(new CSTranslationCAstData(field));
        return newField;
    }
}