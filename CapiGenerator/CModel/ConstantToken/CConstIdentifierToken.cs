using CapiGenerator.CModel.Type;
using CapiGenerator.Parser;
using CapiGenerator.UtilTypes;
using CppAst;

namespace CapiGenerator.CModel.ConstantToken;

public class CConstIdentifierToken : BaseCConstantToken
{
    private readonly ResoleRef<ICConstAssignable, string> _constantRef;
    private ResoleRef<ICType, string>? _castTypeRef;
    private CConstantType _castType = CConstantType.NONE;

    public CConstIdentifierToken(string constIdentifierName, CppSourceLocation debugInfo)
        : base(debugInfo)
    {
        _constantRef = new(constIdentifierName);
    }

    public CConstIdentifierToken(ICConstAssignable constantIdentifier, CppSourceLocation debugInfo)
        : base(debugInfo)
    {
        _constantRef = new(constantIdentifier);
    }

    public ICConstAssignable? GetConstantModel()
    {
        return _constantRef.IsOutputResolved() ? _constantRef.Output : null;
    }

    public void MarkAsCastCandidate()
    {
        if (_constantRef.TryGetKey(out var name) && name is not null)
        {
            _castTypeRef = new(name);
        }
    }

    public bool TryGetCastType(out CConstantType castType)
    {
        castType = _castType;
        return castType != CConstantType.NONE;
    }

    public bool TryGetName(out string? name)
    {
        name = _constantRef.TryGetKey(out var key) ? key : null;
        return name is not null;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        _constantRef.TrySetOutputFromResolver(compilationUnit);
        if (_constantRef.IsOutputResolved() || _castTypeRef is null)
        {
            return;
        }

        var castTypeRef = _castTypeRef.Value;
        castTypeRef.TrySetOutputFromResolver(compilationUnit);
        _castTypeRef = castTypeRef;
        if (castTypeRef.Output is CTypedef typedef)
        {
            typedef.OnSecondPass(compilationUnit);
        }

        if (castTypeRef.Output is { } type)
        {
            _castType = GetConstantType(type);
        }
    }

    private static CConstantType GetConstantType(ICType type)
    {
        if (type is CTypedef typedef)
        {
            return typedef.InnerType.GetCType() is { } innerType
                ? GetConstantType(innerType)
                : CConstantType.Unknown;
        }

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