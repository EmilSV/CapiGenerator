using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntFast16TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntFast16TBuiltinTypedef Instance { get; } = new UIntFast16TBuiltinTypedef();

    public override string Name => "uint16_t";

    private UIntFast16TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_fast16_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedInt };
    }
}