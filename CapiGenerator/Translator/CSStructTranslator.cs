using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
            FixedInlineArrayFields(structItem);
        }
    }

    protected static CSStruct TranslateStruct(CStruct structItem)
    {
        var newCSStruct = new CSStruct
        {
            Name = structItem.Name,
        };

        foreach (var field in structItem.Fields)
        {
            newCSStruct.Fields.Add(TranslateField(field));
            AddNestedAnonymousRecord(newCSStruct, field);
        }

        newCSStruct.EnrichingDataStore.Set(new CSTranslationFromCAstData(structItem));
        structItem.EnrichingDataStore.Set(new CTranslationToCSAstData(newCSStruct));
        return newCSStruct;
    }

    protected static CSStruct TranslateUnionRecord(CUnion unionItem)
    {
        var newCSStruct = new CSStruct
        {
            Name = unionItem.Name,
        };

        newCSStruct.Attributes.Add(CreateExplicitLayoutAttribute());

        foreach (var field in unionItem.Fields)
        {
            var newField = TranslateField(field);
            newField.Attributes.Add(CSAttribute<FieldOffsetAttribute>.Create(
                [0.ToString()],
                []));
            newCSStruct.Fields.Add(newField);
            AddNestedAnonymousRecord(newCSStruct, field);
        }

        newCSStruct.EnrichingDataStore.Set(new CSTranslationFromCAstData(unionItem));
        unionItem.EnrichingDataStore.Set(new CTranslationToCSAstData(newCSStruct));
        return newCSStruct;
    }

    protected static void AddNestedAnonymousRecord(CSStruct parent, CField field)
    {
        var cType = field.GetFieldType().GetCType();
        if (cType is not { IsAnonymous: true })
        {
            return;
        }

        var nestedType = cType switch
        {
            CStruct cStruct => TranslateStruct(cStruct),
            CUnion cUnion => TranslateUnionRecord(cUnion),
            _ => null,
        };

        if (nestedType is not null)
        {
            parent.NestedTypes.Add(nestedType);
        }
    }

    private static CSAttribute<StructLayoutAttribute> CreateExplicitLayoutAttribute()
    {
        return CSAttribute<StructLayoutAttribute>.Create(
            ["System.Runtime.InteropServices.LayoutKind.Explicit"],
            []);
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


    protected static void FixedInlineArrayFields(CSStruct cSStruct)
    {
        foreach (var field in cSStruct.Fields)
        {
            if (field.Type.Modifiers is [CSFixedInlineArrayType arrayType, ..])
            {
                FixedInlineArrayField(field, arrayType);
            }
        }
    }

    protected static void FixedInlineArrayField(CSField field, CSFixedInlineArrayType arrayType)
    {
        if (field.Type.Type is null)
        {
            throw new InvalidOperationException($"Cannot convert fixed inline array field {field.Name} because its element type is not resolved");
        }

        ValidateInlineArraySize(field, arrayType);

        var modifiers = field.Type.GetModifiersAsSpan();
        if (modifiers is not [CSFixedInlineArrayType, ..])
        {
            throw new InvalidOperationException($"Field {field.Name} does not start with a fixed inline array modifier");
        }

        var fixedArrayCount = 0;
        while (fixedArrayCount < modifiers.Length && modifiers[fixedArrayCount] is CSFixedInlineArrayType fixedArrayType)
        {
            ValidateInlineArraySize(field, fixedArrayType);
            fixedArrayCount++;
        }

        var elementType = new CSTypeInstance(field.Type.Type, modifiers[fixedArrayCount..]);
        for (var i = fixedArrayCount - 1; i >= 0; i--)
        {
            var fixedArrayType = (CSFixedInlineArrayType)modifiers[i];
            elementType = new CSTypeInstance(new CSInlineArrayType(fixedArrayType.Size, elementType));
        }

        field.Type = elementType;
    }

    private static void ValidateInlineArraySize(CSField field, CSFixedInlineArrayType arrayType)
    {
        if (arrayType.Size is < CSInlineArrayType.MinSupportedSize or > CSInlineArrayType.MaxSupportedSize)
        {
            throw new NotSupportedException(
                $"Fixed inline array field {field.Name} has size {arrayType.Size}, but only sizes 1 through 16 are supported");
        }
    }
}
