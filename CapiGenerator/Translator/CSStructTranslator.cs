using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
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
        var newCSStruct = new CSStruct(structItem)
        {
            Name = structItem.Name,
        };

        foreach (var nestedType in structItem.NestedTypes)
        {
            AddNestedRecord(newCSStruct, nestedType);
        }

        foreach (var field in structItem.Fields)
        {
            newCSStruct.Fields.Add(TranslateField(field));
        }

        newCSStruct.EnrichingDataStore.Set(new CSTranslationFromCAstData(structItem));
        structItem.EnrichingDataStore.Set(new CTranslationToCSAstData(newCSStruct));
        return newCSStruct;
    }

    protected static CSStruct TranslateUnionRecord(CUnion unionItem)
    {
        var newCSStruct = new CSStruct(unionItem)
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
        }

        foreach (var nestedType in unionItem.NestedTypes)
        {
            AddNestedRecord(newCSStruct, nestedType);
        }

        newCSStruct.EnrichingDataStore.Set(new CSTranslationFromCAstData(unionItem));
        unionItem.EnrichingDataStore.Set(new CTranslationToCSAstData(newCSStruct));
        return newCSStruct;
    }

    protected static void AddNestedRecord(CSStruct parent, ICType cType)
    {
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
        var newField = new CSField(field)
        {
            Name = field.Name,
            Type = csTypeInstance,
        };

        newField.EnrichingDataStore.Set(new CSTranslationFromCAstData(field));
        field.EnrichingDataStore.Set(new CTranslationToCSAstData(newField));

        return newField;
    }


    protected internal static void FixedInlineArrayFields(CSStruct cSStruct)
    {
        var originalNestedTypeCount = cSStruct.NestedTypes.Count;
        for (var i = 0; i < originalNestedTypeCount; i++)
        {
            if (cSStruct.NestedTypes[i] is CSStruct nestedStruct)
            {
                FixedInlineArrayFields(nestedStruct);
            }
        }

        foreach (var field in cSStruct.Fields)
        {
            if (TryCreateFixedBufferFieldType(field, field.Type, out var fixedBufferType, out var fixedBufferSize))
            {
                field.Type = fixedBufferType;
                field.FixedBufferSize = fixedBufferSize;
                cSStruct.IsUnsafe = true;
                continue;
            }

            field.Type = FixedInlineArrayTypeInstance(cSStruct, field, field.Type);
        }
    }

    private static bool TryCreateFixedBufferFieldType(
        CSField field,
        CSTypeInstance typeInstance,
        out CSTypeInstance fixedBufferType,
        out uint fixedBufferSize)
    {
        fixedBufferType = typeInstance;
        fixedBufferSize = 0;

        if (typeInstance.Type is null)
        {
            return false;
        }

        var modifiers = typeInstance.GetModifiersAsSpan();
        if (modifiers is not [CSFixedInlineArrayType, ..])
        {
            return false;
        }

        uint size = 1;
        var fixedArrayEndIndex = 0;
        while (fixedArrayEndIndex < modifiers.Length && modifiers[fixedArrayEndIndex] is CSFixedInlineArrayType fixedArrayType)
        {
            ValidateInlineArraySize(field, fixedArrayType);
            size = checked(size * fixedArrayType.Size);
            fixedArrayEndIndex++;
        }

        var elementType = new CSTypeInstance(typeInstance.Type, modifiers[fixedArrayEndIndex..]);
        if (!IsFixedBufferPrimitiveType(elementType))
        {
            return false;
        }

        fixedBufferType = elementType;
        fixedBufferSize = size;
        return true;
    }

    private static bool IsFixedBufferPrimitiveType(CSTypeInstance typeInstance)
    {
        if (typeInstance.Modifiers.Count != 0 || typeInstance.Type is not CSPrimitiveType primitiveType)
        {
            return false;
        }

        return primitiveType.KindValue is
            CSPrimitiveType.Kind.Bool or
            CSPrimitiveType.Kind.Byte or
            CSPrimitiveType.Kind.SByte or
            CSPrimitiveType.Kind.Short or
            CSPrimitiveType.Kind.UShort or
            CSPrimitiveType.Kind.Int or
            CSPrimitiveType.Kind.UInt or
            CSPrimitiveType.Kind.Long or
            CSPrimitiveType.Kind.ULong or
            CSPrimitiveType.Kind.Char or
            CSPrimitiveType.Kind.Float or
            CSPrimitiveType.Kind.Double;
    }

    protected static CSTypeInstance FixedInlineArrayTypeInstance(
        CSStruct parentStruct,
        CSField field,
        CSTypeInstance typeInstance)
    {
        if (typeInstance.Type is null)
        {
            if (typeInstance.Modifiers.Any(modifier => modifier is CSFixedInlineArrayType))
            {
                throw new InvalidOperationException($"Cannot convert fixed inline array field {field.Name} because its element type is not resolved");
            }

            return typeInstance;
        }

        if (typeInstance.Type is CSUnmanagedFunctionType functionType)
        {
            functionType.ReplaceTypeInstances(nestedTypeInstance => FixedInlineArrayTypeInstance(
                parentStruct,
                field,
                nestedTypeInstance));
        }

        var modifiers = typeInstance.GetModifiersAsSpan();
        var firstFixedArrayIndex = IndexOfModifier<CSFixedInlineArrayType>(modifiers);
        if (firstFixedArrayIndex < 0)
        {
            return typeInstance;
        }

        var fixedArrayEndIndex = firstFixedArrayIndex;
        while (fixedArrayEndIndex < modifiers.Length && modifiers[fixedArrayEndIndex] is CSFixedInlineArrayType fixedArrayType)
        {
            ValidateInlineArraySize(field, fixedArrayType);
            fixedArrayEndIndex++;
        }

        var elementType = FixedInlineArrayTypeInstance(parentStruct, field, new CSTypeInstance(
            typeInstance.Type,
            modifiers[fixedArrayEndIndex..]));
        for (var i = fixedArrayEndIndex - 1; i >= firstFixedArrayIndex; i--)
        {
            var fixedArrayType = (CSFixedInlineArrayType)modifiers[i];
            elementType = CreateInlineArrayType(parentStruct, field, fixedArrayType, elementType);
        }

        if (firstFixedArrayIndex == 0)
        {
            return elementType;
        }

        if (elementType.Type is null)
        {
            throw new InvalidOperationException($"Cannot convert fixed inline array field {field.Name} because its converted array type is not resolved");
        }

        return new CSTypeInstance(elementType.Type, modifiers[..firstFixedArrayIndex]);
    }

    private static int IndexOfModifier<TModifier>(ReadOnlySpan<BaseCSTypeModifier> modifiers)
        where TModifier : BaseCSTypeModifier
    {
        for (var i = 0; i < modifiers.Length; i++)
        {
            if (modifiers[i] is TModifier)
            {
                return i;
            }
        }

        return -1;
    }

    private static CSTypeInstance CreateInlineArrayType(
        CSStruct parentStruct,
        CSField field,
        CSFixedInlineArrayType fixedArrayType,
        CSTypeInstance elementType)
    {
        if (fixedArrayType.Size <= CSInlineArrayType.MaxBuiltInSize)
        {
            return new CSTypeInstance(new CSInlineArrayType(fixedArrayType.Size, elementType));
        }

        var inlineArrayStruct = CreateNestedInlineArrayStruct(parentStruct, field, fixedArrayType.Size, elementType);
        parentStruct.NestedTypes.Add(inlineArrayStruct);
        return new CSTypeInstance(inlineArrayStruct);
    }

    private static CSStruct CreateNestedInlineArrayStruct(
        CSStruct parentStruct,
        CSField field,
        uint size,
        CSTypeInstance elementType)
    {
        var inlineArrayStruct = new CSStruct
        {
            Name = CreateNestedInlineArrayStructName(parentStruct, field, size),
            AccessModifier = CSAccessModifier.Public,
            IsUnsafe = RequiresUnsafe(elementType),
        };

        inlineArrayStruct.Attributes.Add(CSAttribute<InlineArrayAttribute>.Create(
            [size.ToString()],
            []));
        inlineArrayStruct.Fields.Add(new CSField
        {
            Name = "_element0",
            Type = elementType,
            AccessModifier = CSAccessModifier.Private,
        });

        return inlineArrayStruct;
    }

    private static string CreateNestedInlineArrayStructName(CSStruct parentStruct, CSField field, uint size)
    {
        var baseName = $"{ToPascalCaseIdentifier(field.Name)}InlineArray{size}";
        var name = baseName;
        var suffix = 1;

        while (parentStruct.NestedTypes.Any(nestedType => nestedType.Name == name))
        {
            name = $"{baseName}_{suffix}";
            suffix++;
        }

        return name;
    }

    private static string ToPascalCaseIdentifier(string name)
    {
        var builder = new StringBuilder();
        var capitalizeNext = true;

        foreach (var character in name)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(capitalizeNext ? char.ToUpperInvariant(character) : character);
                capitalizeNext = false;
            }
            else
            {
                capitalizeNext = true;
            }
        }

        if (builder.Length == 0 || char.IsDigit(builder[0]))
        {
            builder.Insert(0, "FixedArray");
        }

        return builder.ToString();
    }

    private static bool RequiresUnsafe(CSTypeInstance type)
    {
        return type.Modifiers.Any(modifier => modifier is CsPointerType) ||
            type.Type is CSInlineArrayType inlineArrayType && RequiresUnsafe(inlineArrayType.ElementType) ||
            type.Type is CSStruct csStruct && csStruct.IsUnsafe;
    }

    private static void ValidateInlineArraySize(CSField field, CSFixedInlineArrayType arrayType)
    {
        if (arrayType.Size < CSInlineArrayType.MinSupportedSize)
        {
            throw new NotSupportedException(
                $"Fixed inline array field {field.Name} has size {arrayType.Size}, but only sizes greater than or equal to 1 are supported");
        }
    }
}
