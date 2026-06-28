using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel.BuiltinTypedefs;
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

        if (CTypeRef.Output is BaseCAnonymousType and ICSecondPassable secondPassable)
        {
            secondPassable.OnSecondPass(compilationUnit);
        }
    }

    public static CTypeInstance FromCppType(CppType type)
    {
        var (convertedType, modifiers) = type.UnpackModifiers();
        if (convertedType is CppTypedef cppTypedef)
        {
            var builtinTypedef = AllBuiltinTypedefs.AllTypedefs.FirstOrDefault(item => item.Name == cppTypedef.Name);
            if (builtinTypedef is not null)
            {
                return new CTypeInstance(builtinTypedef, modifiers);
            }
        }

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
