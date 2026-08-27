using CapiGenerator.CModel.Type;
using CapiGenerator.Parser;
using CapiGenerator.UtilTypes;
using CppAst;

namespace CapiGenerator.CModel.ConstantToken;

public sealed class CConstCastToken : BaseCConstantToken
{
    private readonly ResoleRef<ICType, string> _typeRef;
    private CConstantType _constantType;

    public CConstCastToken(ICType type, CppSourceLocation sourceLocation)
        : base(sourceLocation)
    {
        _typeRef = new(type);
        _constantType = GetConstantType(type);
    }

    public CConstCastToken(string typeName, CppSourceLocation sourceLocation)
        : base(sourceLocation)
    {
        _typeRef = new(typeName);
        _constantType = typeName == "size_t" ? CConstantType.Size_t : CConstantType.NONE;
    }

    public bool TryGetConstantType(out CConstantType constantType)
    {
        constantType = _constantType;
        return constantType is not CConstantType.NONE and not CConstantType.Unknown;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (TryGetConstantType(out _))
        {
            return;
        }

        _typeRef.TrySetOutputFromResolver(compilationUnit);
        if (_typeRef.Output is { } type)
        {
            _constantType = GetConstantType(type, compilationUnit, []);
        }
    }

    private static CConstantType GetConstantType(
        ICType type,
        CCompilationUnit compilationUnit,
        HashSet<CTypedef> visitedTypedefs)
    {
        if (type is CTypedef typedef)
        {
            if (!visitedTypedefs.Add(typedef))
            {
                return CConstantType.Unknown;
            }

            typedef.OnSecondPass(compilationUnit);
            return typedef.InnerType.GetCType() is { } innerType
                ? GetConstantType(innerType, compilationUnit, visitedTypedefs)
                : CConstantType.Unknown;
        }

        return GetConstantType(type);
    }

    private static CConstantType GetConstantType(ICType type)
    {

        if (type is CPrimitiveType primitiveType)
        {
            return primitiveType.KindValue switch
            {
                CPrimitiveType.Kind.Char => CConstantType.Char,
                CPrimitiveType.Kind.SignedChar => CConstantType.Int8_t,
                CPrimitiveType.Kind.UnsignedChar => CConstantType.UInt8_t,
                CPrimitiveType.Kind.Short => CConstantType.Short,
                CPrimitiveType.Kind.UnsignedShortInt => CConstantType.UnsignedShort,
                CPrimitiveType.Kind.Int => CConstantType.Int,
                CPrimitiveType.Kind.UnsignedInt => CConstantType.UnsignedInt,
                CPrimitiveType.Kind.Long => CConstantType.Long,
                CPrimitiveType.Kind.UnsignedLong => CConstantType.UnsignedLong,
                CPrimitiveType.Kind.LongLong => CConstantType.LongLong,
                CPrimitiveType.Kind.UnsignedLongLong => CConstantType.UnsignedLongLong,
                CPrimitiveType.Kind.Float => CConstantType.Float,
                CPrimitiveType.Kind.Double => CConstantType.Double,
                _ => CConstantType.Unknown,
            };
        }

        return type.Name switch
        {
            "int8_t" or "int_fast8_t" or "int_least8_t" => CConstantType.Int8_t,
            "uint8_t" or "uint_fast8_t" or "uint_least8_t" => CConstantType.UInt8_t,
            "int16_t" or "int_fast16_t" or "int_least16_t" => CConstantType.Int16_t,
            "uint16_t" or "uint_fast16_t" or "uint_least16_t" => CConstantType.UInt16_t,
            "int32_t" or "int_fast32_t" or "int_least32_t" => CConstantType.Int32_t,
            "uint32_t" or "uint_fast32_t" or "uint_least32_t" => CConstantType.UInt32_t,
            "int64_t" or "int_fast64_t" or "int_least64_t" or "intmax_t" => CConstantType.Int64_t,
            "uint64_t" or "uint_fast64_t" or "uint_least64_t" or "uintmax_t" => CConstantType.UInt64_t,
            "intptr_t" or "ptrdiff_t" => CConstantType.IntPtr_t,
            "uintptr_t" => CConstantType.UIntPtr_t,
            "size_t" => CConstantType.Size_t,
            _ when type is CEnum => CConstantType.Int,
            _ => CConstantType.Unknown,
        };
    }
}
