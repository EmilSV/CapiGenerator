using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UInt8TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UInt8TBuiltinTypedef Instance { get; } = new UInt8TBuiltinTypedef();

    public override string Name => "uint8_t";

    private UInt8TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedChar };
    }
}