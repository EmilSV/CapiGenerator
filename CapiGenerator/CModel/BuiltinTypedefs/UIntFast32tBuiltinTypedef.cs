using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntFast32tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntFast32tBuiltinTypedef Instance { get; } = new UIntFast32tBuiltinTypedef();

    public override string Name => "uint_fast32_t";

    private UIntFast32tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_fast32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedInt };
    }
}