using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ClangSharp.Interop;
using CppAst;

namespace CapiGenerator.CModel.Type;

public static class TypeConverter
{
    public static CTypeInstance PartialConvert(Guid compilationUnitId, CppType type)
    {
        var cachedModifiersList = new List<CTypeModifier>();
        List<CTypeModifier> modifiers = cachedModifiersList;
        modifiers.Clear();

        CppType convertedType = UnpackModifiers(type, modifiers, null);
        if (TryConvertToCType(convertedType, out var cType))
        {
            return new CTypeInstance(compilationUnitId, cType, CollectionsMarshal.AsSpan(modifiers));
        }
        else
        {
            return new CTypeInstance(compilationUnitId, convertedType.FullName, CollectionsMarshal.AsSpan(modifiers));
        }
    }

    private static CppType UnpackModifiers(CppType type, List<CTypeModifier> modifiers, CTypeModifier? modifierToAdd)
    {
        if (modifierToAdd != null)
        {
            modifiers.Add(modifierToAdd);
        }

        return type switch
        {
            CppPointerType pointerType => UnpackModifiers(pointerType.ElementType, modifiers, PointerType.Instance),
            CppArrayType arrayType => UnpackModifiers(arrayType.ElementType, modifiers, new ArrayType(arrayType.Size)),
            CppTypedef => type,
            CppFunctionType => type,
            CppPrimitiveType => type,
            CppEnum => type,
            CppClass => type,
            _ => throw new ArgumentException($"unsupported type {type.GetType().Name}", nameof(type))
        };
    }

    private static bool TryConvertToCType(CppType type, [NotNullWhen(true)] out ICType? cType)
    { 
        cType = type switch
        {
            CppPrimitiveType cppPrimitiveType => CPrimitiveType.FromCppPrimitiveType(cppPrimitiveType),
            CppFunctionType cppFunctionType => AnonymousFunctionType.FromCFunctionType(Guid.Empty, cppFunctionType),
            _ => null,
        };

        return cType != null;
    }
}