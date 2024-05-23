using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntLeast32tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntLeast32tBuiltinTypedef Instance { get; } = new UIntLeast32tBuiltinTypedef();

    public override string Name => "uint_least32_t";

    private UIntLeast32tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_least32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedInt };
    }
}