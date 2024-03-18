using System.Diagnostics.CodeAnalysis;
using CapiGenerator.Parser;
using CppAst;

namespace CapiGenerator.CModel.Type;

public class CTypeInstance : BaseCAstItem
{
    private readonly CTypeModifier[]? _modifiers;
    public ResoleRef<ICType, string> CTypeRef { get; }
    public ReadOnlySpan<CTypeModifier> Modifiers =>
        _modifiers ?? ReadOnlySpan<CTypeModifier>.Empty;

    public CTypeInstance(Guid compilationUnitId, ICType cType, ReadOnlySpan<CTypeModifier> modifiers)
        : base(compilationUnitId)
    {
        CTypeRef = new(cType);
        _modifiers = modifiers.ToArray();
    }

    public CTypeInstance(Guid compilationUnitId, string typeName, ReadOnlySpan<CTypeModifier> modifiers)
        : base(compilationUnitId)
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
        if (compilationUnit.CompilationUnitId != CompilationUnitId)
        {
            throw new InvalidOperationException("Compilation unit id mismatch");
        }

        CTypeRef.TrySetOutputFromResolver(compilationUnit);
    }

    public static CTypeInstance FromCppType(CppType type, Guid compilationUnitId)
    {
        var (convertedType, modifiers) = UnpackModifiers(type);
        if (TryConvertToCType(convertedType, out var cType))
        {
            return new CTypeInstance(compilationUnitId, cType, modifiers);
        }
        else
        {
            return new CTypeInstance(compilationUnitId, convertedType.FullName, modifiers);
        }
    }

    private static (CppType finalType, CTypeModifier[] modifiers) UnpackModifiers(CppType type)
    {
        List<CTypeModifier> modifiers = [];
        CppType? nextType = type;

        static CppType HandlePointerType(CppPointerType type, List<CTypeModifier> outModifiers)
        {
            outModifiers.Add(PointerType.Instance);
            return type;
        }

        static CppType HandleArrayType(CppArrayType type, List<CTypeModifier> outModifiers)
        {
            outModifiers.Add(new ArrayType(type.Size));
            return type.ElementType;
        }

        do
        {
            type = nextType;
            nextType = nextType switch
            {
                CppPointerType pointerType => HandlePointerType(pointerType, modifiers),
                CppArrayType arrayType => HandleArrayType(arrayType, modifiers),
                CppTypedef => null,
                CppFunctionType => null,
                CppPrimitiveType => null,
                CppEnum => null,
                CppClass => null,
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
            CppFunctionType cppFunctionType => AnonymousFunctionType.FromCFunctionType(Guid.Empty, cppFunctionType),
            _ => null,
        };

        return cType != null;
    }
}
