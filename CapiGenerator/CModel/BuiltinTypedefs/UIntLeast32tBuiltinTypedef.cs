using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UInt32tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UInt32tBuiltinTypedef Instance { get; } = new UInt32tBuiltinTypedef();

    public override string Name => "uint32_t";

    private UInt32tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedInt };
    }
}