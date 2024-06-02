using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntFast64TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntFast64TBuiltinTypedef Instance { get; } = new UIntFast64TBuiltinTypedef();

    public override string Name => "uint_fast64_t";

    private UIntFast64TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_fast64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedLongLong };
    }
}