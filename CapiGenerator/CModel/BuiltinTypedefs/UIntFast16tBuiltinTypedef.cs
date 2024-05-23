using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntFast16tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntFast16tBuiltinTypedef Instance { get; } = new UIntFast16tBuiltinTypedef();

    public override string Name => "uint16_t";

    private UIntFast16tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_fast16_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedInt };
    }
}