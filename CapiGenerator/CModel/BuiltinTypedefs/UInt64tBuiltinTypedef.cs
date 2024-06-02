using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UInt64TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UInt64TBuiltinTypedef Instance { get; } = new UInt64TBuiltinTypedef();

    public override string Name => "uint64_t";

    private UInt64TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedLongLong };
    }
}