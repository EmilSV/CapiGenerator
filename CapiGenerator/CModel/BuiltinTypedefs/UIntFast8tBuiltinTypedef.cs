using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntFast8TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntFast8TBuiltinTypedef Instance { get; } = new UIntFast8TBuiltinTypedef();

    public override string Name => "uint8_t";

    private UIntFast8TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_fast8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedChar};
    }
}