using System.Diagnostics.CodeAnalysis;
using CapiGenerator.Parser;
using CapiGenerator.UtilTypes;
using CppAst;

namespace CapiGenerator.CModel.Type;

public class CTypeInstance : BaseCAstItem
{
    private readonly CTypeModifier[]? _modifiers;
    public ResoleRef<ICType, string> CTypeRef { get; }
    public ReadOnlySpan<CTypeModifier> Modifiers =>
        _modifiers ?? ReadOnlySpan<CTypeModifier>.Empty;

    public CTypeInstance(ICType cType, ReadOnlySpan<CTypeModifier> modifiers)
    {
        CTypeRef = new(cType);
        _modifiers = modifiers.ToArray();
    }

    public CTypeInstance(string typeName, ReadOnlySpan<CTypeModifier> modifiers)
    {
        CTypeRef = new(typeName);
        _modifiers = modifiers.ToArray();
    }

    public bool GetIsCompletedType()
    {
        return CTypeRef.IsOutputResolved();
    }

    public ICType? GetCType()
    {
        return CTypeRef.Output;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        CTypeRef.TrySetOutputFromResolver(compilationUnit);

        if (CTypeRef.Output is { IsAnonymous: true } and ICSecondPassable secondPassable)
        {
            secondPassable.OnSecondPass(compilationUnit);
        }
    }

    public static CTypeInstance FromCppType(CppType type)
    {
        var (convertedType, modifiers) = UnpackModifiers(type);
        if (TryConvertToCType(convertedType, out var cType))
        {
            return new CTypeInstance(cType, modifiers);
        }
        else
        {
            string typeName = convertedType switch
            {
                CppTypedef typedef => typedef.Name,
                CppPrimitiveType primitiveType => primitiveType.FullName,
                CppEnum enumType => enumType.Name,
                CppClass classType => classType.Name,
                _ => throw new ArgumentException($"unsupported type {convertedType.GetType().Name}", nameof(type))
            };
            return new CTypeInstance(typeName, modifiers);
        }
    }

    private static (CppType finalType, CTypeModifier[] modifiers) UnpackModifiers(CppType type)
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

    private static bool TryConvertToCType(CppType type, [NotNullWhen(true)] out ICType? cType)
    {
        cType = type switch
        {
            CppPrimitiveType cppPrimitiveType => CPrimitiveType.FromCppPrimitiveType(cppPrimitiveType),
            CppFunctionType cppFunctionType => AnonymousFunctionType.FromCFunctionType(cppFunctionType),
            _ => null,
        };

        return cType != null;
    }
}
