using CapiGenerator.CModel;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSStructTranslator : BaseTranslator
{
    public override void FirstPass(
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseCSTypeResolver typeResolver,
        BaseTranslatorOutputChannel outputChannel)
    {
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var structItem in compilationUnit.GetStructEnumerable())
            {
                outputChannel.OnReceiveStruct(TranslateStruct(structItem, typeResolver));
            }
        }
    }

    private static CSStruct TranslateStruct(CStruct structItem, BaseCSTypeResolver typeResolver)
    {
        List<CSField> fields = [];

        foreach (var field in structItem.Fields)
        {
            fields.Add(TranslateField(field, typeResolver));
        }

        var newCSStruct = new CSStruct(structItem.Name, fields.ToArray(), []);
        newCSStruct.EnrichingDataStore.Add(new CSTranslationFromCAstData(structItem));
        structItem.EnrichingDataStore.Add(new CSTranslationsTypeData(newCSStruct));
        return newCSStruct;
    }

    private static CSField TranslateField(CField field, BaseCSTypeResolver typeResolver)
    {
        CSResolveType fieldType = new(field.GetFieldType(), typeResolver);
        var newField = new CSField(field.Name, fieldType, null);
        newField.EnrichingDataStore.Add(new CSTranslationFromCAstData(field));
        return newField;
    }
}