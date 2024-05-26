using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UInt32TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UInt32TBuiltinTypedef Instance { get; } = new UInt32TBuiltinTypedef();

    public override string Name => "uint32_t";

    private UInt32TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedInt };
    }
}