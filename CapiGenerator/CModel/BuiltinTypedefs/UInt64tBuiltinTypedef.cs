using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UInt64tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UInt64tBuiltinTypedef Instance { get; } = new UInt64tBuiltinTypedef();

    public override string Name => "uint64_t";

    private UInt64tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedLongLong};
    }
}