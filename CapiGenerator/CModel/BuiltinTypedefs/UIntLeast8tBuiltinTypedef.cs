using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UInt8tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UInt8tBuiltinTypedef Instance { get; } = new UInt8tBuiltinTypedef();

    public override string Name => "uint8_t";

    private UInt8tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedChar};
    }
}