using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UInt16TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UInt16TBuiltinTypedef Instance { get; } = new UInt16TBuiltinTypedef();

    public override string Name => "uint16_t";

    private UInt16TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint16_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedShort };
    }
}