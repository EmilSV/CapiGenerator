using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntFast32TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntFast32TBuiltinTypedef Instance { get; } = new UIntFast32TBuiltinTypedef();

    public override string Name => "uint_fast32_t";

    private UIntFast32TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_fast32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedInt };
    }
}