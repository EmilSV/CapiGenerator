using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntLeast64TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntLeast64TBuiltinTypedef Instance { get; } = new UIntLeast64TBuiltinTypedef();

    public override string Name => "uint_least64_t";

    private UIntLeast64TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_least64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedLongLong};
    }
}