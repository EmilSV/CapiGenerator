using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntFast8tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntFast8tBuiltinTypedef Instance { get; } = new UIntFast8tBuiltinTypedef();

    public override string Name => "uint8_t";

    private UIntFast8tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_fast8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedChar};
    }
}