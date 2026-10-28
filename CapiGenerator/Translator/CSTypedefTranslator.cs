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
        var typedefStructs = inputChannel.GetStructs();

        foreach (var structItem in typedefStructs)
        {
            structItem.OnSecondPass(translationUnit);
            CSStructTranslator.FixedInlineArrayFields(structItem);
            CSStructTranslator.MarkUnsafe(structItem);
        }

        var typedefStructSet = typedefStructs.ToArray().ToHashSet();
        foreach (var structItem in typedefStructs)
        {
            AddImplicitConversions(structItem, typedefStructSet);
        }
    }

    private static void AddImplicitConversions(CSStruct typedefStruct, HashSet<CSStruct> typedefStructs)
    {
        var innerType = GetConvertibleValueType(typedefStruct);

        while (innerType is not null)
        {
            typedefStruct.Methods.Add(new CSMethod(
                CSClassMemberModifier.Public | CSClassMemberModifier.Static | CSClassMemberModifier.Implicit,
                new CSTypeInstance(typedefStruct),
                [(CSTypeInstance.CopyWithNewType(innerType, innerType.Type!), "value")])
            {
                Body = " => new() { Value = value };"
            });
            typedefStruct.Methods.Add(new CSMethod(
                CSClassMemberModifier.Public | CSClassMemberModifier.Static | CSClassMemberModifier.Implicit,
                CSTypeInstance.CopyWithNewType(innerType, innerType.Type!),
                [(new CSTypeInstance(typedefStruct), "value")])
            {
                Body = " => value.Value;"
            });

            if (CSStructTranslator.RequiresUnsafe(innerType))
            {
                typedefStruct.IsUnsafe = true;
            }

            innerType = innerType.Modifiers.Count == 0 &&
                innerType.Type is CSStruct innerStruct &&
                typedefStructs.Contains(innerStruct)
                ? GetConvertibleValueType(innerStruct)
                : null;
        }
    }

    private static CSTypeInstance? GetConvertibleValueType(CSStruct typedefStruct)
    {
        if (typedefStruct.Fields.Count != 1)
        {
            return null;
        }

        var valueField = typedefStruct.Fields[0];
        if (valueField.FixedBufferSize is not null || valueField.Type.Type is null)
        {
            return null;
        }

        if (valueField.Type.Modifiers.Count == 0 &&
            valueField.Type.Type is CSPrimitiveType { KindValue: CSPrimitiveType.Kind.Void or CSPrimitiveType.Kind.Object })
        {
            return null;
        }

        return valueField.Type;
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
        var newCSStruct = new CSStruct(typedefItem)
        {
            Name = typedefItem.Name,
        };
        var valueField = new CSField(typedefItem)
        {
            Name = "Value",
            Type = CSTypeInstance.CreateFromCTypeInstance(typedefItem.InnerType)
        };
        newCSStruct.Fields.Add(valueField);

        typedefItem.AddDerivative(newCSStruct);
        typedefItem.AddDerivative(valueField);
        return newCSStruct;
    }
}
