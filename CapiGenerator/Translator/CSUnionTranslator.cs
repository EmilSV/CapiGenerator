using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSUnionTranslator : CSStructTranslator
{
    public override void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var unionItem in compilationUnit.GetUnionEnumerable())
            {
                if (translationUnit.IsTypeTranslated(unionItem))
                {
                    continue;
                }

                outputChannel.OnReceiveStruct(TranslateUnion(unionItem));
            }
        }
    }

    protected static CSStruct TranslateUnion(CUnion unionItem)
    {
        List<CSField> fields = [];

        foreach (var field in unionItem.Fields)
        {
            var newField = TranslateField(field);
            newField.Attributes.Add(CSAttribute<FieldOffsetAttribute>.Create(
                [field.Offset.ToString()],
                []));
            fields.Add(newField);
        }

        var newCSStruct = new CSStruct
        {
            Name = unionItem.Name,
        };

        newCSStruct.Attributes.Add(CreateStructLayoutAttribute());
        newCSStruct.Fields.AddRange(fields);

        newCSStruct.EnrichingDataStore.Set(new CSTranslationFromCAstData(unionItem));
        unionItem.EnrichingDataStore.Set(new CTranslationToCSAstData(newCSStruct));
        return newCSStruct;
    }

    private static CSAttribute<StructLayoutAttribute> CreateStructLayoutAttribute()
    {
        return CSAttribute<StructLayoutAttribute>.Create(
            ["System.Runtime.InteropServices.LayoutKind.Explicit"],
            []);
    }
}
