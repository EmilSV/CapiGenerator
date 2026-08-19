using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Extensions;
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

                outputChannel.OnReceiveStruct(TranslateStruct(structItem, compilationUnit));
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

    protected static CSStruct TranslateStruct(CStruct structItem, CCompilationUnit compilationUnit)
    {
        var newCSStruct = new CSStruct(structItem)
        {
            Name = structItem.Name,
            Comments = CCommentTranslator.Translate(structItem.Comment, compilationUnit),
        };

        foreach (var nestedType in structItem.NestedTypes)
        {
            AddNestedRecord(newCSStruct, nestedType, compilationUnit);
        }

        foreach (var field in structItem.Fields)
        {
            newCSStruct.Fields.Add(TranslateField(field, compilationUnit));
        }

        structItem.AddDerivative(newCSStruct);
        return newCSStruct;
    }

    protected static CSStruct TranslateUnionRecord(CUnion unionItem, CCompilationUnit compilationUnit)
    {
        var newCSStruct = new CSStruct(unionItem)
        {
            Name = unionItem.Name,
        };

        newCSStruct.Attributes.Add(CreateExplicitLayoutAttribute());

        foreach (var field in unionItem.Fields)
        {
            var newField = TranslateField(field, compilationUnit);
            newField.Attributes.Add(CSAttribute<FieldOffsetAttribute>.Create(
                [0.ToString()],
                []));
            newCSStruct.Fields.Add(newField);
        }

        foreach (var nestedType in unionItem.NestedTypes)
        {
            AddNestedRecord(newCSStruct, nestedType, compilationUnit);
        }

        unionItem.AddDerivative(newCSStruct);
        return newCSStruct;
    }

    protected static void AddNestedRecord(CSStruct parent, ICType cType, CCompilationUnit compilationUnit)
    {
        var nestedType = cType switch
        {
            CStruct cStruct => TranslateStruct(cStruct, compilationUnit),
            CUnion cUnion => TranslateUnionRecord(cUnion, compilationUnit),
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

    protected static CSField TranslateField(CField field, CCompilationUnit compilationUnit)
    {
        var cTypeInstance = field.GetFieldType();
        var csTypeInstance = CSTypeInstance.CreateFromCTypeInstance(cTypeInstance);
        var newField = new CSField(field)
        {
            Name = field.Name,
            Type = csTypeInstance,
            Comments = CCommentTranslator.Translate(field.Comments, compilationUnit),
        };

        field.AddDerivative(newField);

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
            }
            else
            {
                field.Type = FixedInlineArrayTypeInstance(cSStruct, field, field.Type);
            }
        }
    }

    private static bool TryCreateFixedBufferFieldType(
        CSField field,
        CSTypeInstance typeInstance,
        [NotNullWhen(true)] out CSTypeInstance? fixedBufferType,
        out uint fixedBufferSize)
    {

        static bool IsFixedBufferPrimitiveType(ICSType csType) =>
            csType is CSPrimitiveType { KindValue: var kind } && kind is
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

        fixedBufferType = default;
        fixedBufferSize = default;

        if (typeInstance.Type is null)
        {
            return false;
        }

        var modifiers = typeInstance.Modifiers;

        if (modifiers is not [CSFixedInlineArrayType, ..] ||
            modifiers.Any(static m => m is not CSFixedInlineArrayType))
        {
            return false;
        }

        if (!IsFixedBufferPrimitiveType(typeInstance.Type))
        {
            return false;
        }

        uint size = 1;
        foreach (var modifier in modifiers)
        {
            if (modifier is not CSFixedInlineArrayType fixedArrayType)
            {
                throw new InvalidOperationException($"Expected CSFixedInlineArrayType, got {modifier.GetType().Name}");
            }
            size = checked(size * fixedArrayType.Size);
        }

        var elementType = new CSTypeInstance(typeInstance.Type);

        fixedBufferType = elementType;
        fixedBufferSize = size;
        return true;
    }

    protected static CSTypeInstance FixedInlineArrayTypeInstance(
        CSStruct parentStruct,
        CSField field,
        CSTypeInstance typeInstance)
    {
        static bool TryGetFixedInlineRange(ReadOnlySpan<BaseCSTypeModifier> modifiers, out Range range)
        {
            int start = -1;
            int end = -1;

            for (int i = 0; i < modifiers.Length; i++)
            {
                if (modifiers[i] is CSFixedInlineArrayType)
                {
                    if (start == -1)
                    {
                        start = i;
                    }
                    end = i;
                }
            }

            if (start == -1)
            {
                range = default;
                return false;
            }

            range = new Range(start, end + 1);
            return true;
        }

        static string CreateNestedInlineArrayStructName(CSStruct parentStruct, CSField field, uint size)
        {
            var baseName = $"{field.Name.ToPascalCaseIdentifier()}InlineArray{size}";
            var name = baseName;
            var suffix = 1;

            while (parentStruct.NestedTypes.Any(nestedType => nestedType.Name == name))
            {
                name = $"{baseName}_{suffix}";
                suffix++;
            }

            return name;
        }

        static CSStruct CreateNestedInlineArrayStruct(
           CSStruct parentStruct,
           CSField field,
           uint size,
           CSTypeInstance elementType)
        {
            static bool RequiresUnsafe(CSTypeInstance type)
            {
                return type.Modifiers.Any(modifier => modifier is CsPointerType) ||
                    type.Type is CSInlineArrayType inlineArrayType && RequiresUnsafe(inlineArrayType.ElementType) ||
                    type.Type is CSStruct csStruct && csStruct.IsUnsafe;
            }

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

        static CSTypeInstance CreateInlineArrayType(
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

        if (!TryGetFixedInlineRange(modifiers, out var inlineArrayRange))
        {
            return typeInstance;
        }

        var elementType = FixedInlineArrayTypeInstance(parentStruct, field, new CSTypeInstance(
            typeInstance.Type,
            modifiers[inlineArrayRange.End..]));

        foreach (var modifier in modifiers[inlineArrayRange])
        {
            var fixedArrayType = (CSFixedInlineArrayType)modifier;
            elementType = CreateInlineArrayType(parentStruct, field, fixedArrayType, elementType);
        }

        if (elementType.Type is null)
        {
            throw new InvalidOperationException($"Cannot convert fixed inline array field {field.Name} because its converted array type is not resolved");
        }

        return new CSTypeInstance(elementType.Type, modifiers[..inlineArrayRange.Start]);
    }

}
