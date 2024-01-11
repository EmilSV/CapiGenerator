using System.Runtime.InteropServices;
using CppAst;

namespace CapiGenerator.Type;

public static class TypeConverter
{
    [ThreadStatic] private static List<TypeModifier>? cachedModifiersList; 
    public static TypeInstance PartialConvert(CppType type)
    {
        cachedModifiersList ??= [];
        List<TypeModifier> modifiers = cachedModifiersList;
        modifiers.Clear();

        CppType convertedType = InnerConvert(type, modifiers, null);
        return new TypeInstance(convertedType.FullName,  CollectionsMarshal.AsSpan(modifiers));
    }

    private static CppType InnerConvert(CppType type, List<TypeModifier> modifiers, TypeModifier? modifierToAdd)
    {
        if (modifierToAdd != null)
        {
            modifiers.Add(modifierToAdd);
        }

        return type switch
        {
            CppPointerType pointerType => InnerConvert(pointerType.ElementType, modifiers, PointerType.Instance),
            CppArrayType arrayType => InnerConvert(arrayType.ElementType, modifiers, new ArrayType(arrayType.Size)),
            CppTypedef => type,
            CppFunctionType => type,
            CppPrimitiveType => type,
            CppEnum => type,
            CppClass => type,
            _ => throw new ArgumentException($"unsupported type {type.GetType().Name}" ,nameof(type))
        };
    }
}