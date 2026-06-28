using CapiGenerator.CModel.Type;
using CppAst;

namespace CapiGenerator.CModel;


public static class CppTypeExtension
{

    public static (CppType finalType, CTypeModifier[] modifiers) UnpackModifiers(this CppType type)
    {
        List<CTypeModifier> modifiers = [];
        CppType? nextType = type;

        static CppType HandlePointerType(CppPointerType type, List<CTypeModifier> outModifiers)
        {
            outModifiers.Add(PointerType.Instance);
            return type.ElementType;
        }

        static CppType HandleArrayType(CppArrayType type, List<CTypeModifier> outModifiers)
        {
            outModifiers.Add(new ArrayType(type.Size));
            return type.ElementType;
        }

        static CppType HandleQualifiedType(CppQualifiedType type, List<CTypeModifier> _)
        {
            return type.ElementType;
        }

        do
        {
            type = nextType;
            nextType = nextType switch
            {
                CppPointerType pointerType => HandlePointerType(pointerType, modifiers),
                CppArrayType arrayType => HandleArrayType(arrayType, modifiers),
                CppQualifiedType qualifiedType => HandleQualifiedType(qualifiedType, modifiers),
                CppTypedef or CppFunctionType or CppFunctionType => null,
                CppPrimitiveType or CppEnum or CppClass => null,
                _ => throw new ArgumentException($"unsupported type {type.GetType().Name}", nameof(type))
            };
        } while (nextType != null);

        return (type, modifiers.ToArray());
    }
}
