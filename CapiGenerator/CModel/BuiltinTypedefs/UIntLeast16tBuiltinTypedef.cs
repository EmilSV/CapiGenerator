using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UInt16tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UInt16tBuiltinTypedef Instance { get; } = new UInt16tBuiltinTypedef();

    public override string Name => "uint16_t";

    private UInt16tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint16_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedShort };
    }
}