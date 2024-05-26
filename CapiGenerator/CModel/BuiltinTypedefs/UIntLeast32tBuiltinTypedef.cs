using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntLeast32TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntLeast32TBuiltinTypedef Instance { get; } = new UIntLeast32TBuiltinTypedef();

    public override string Name => "uint_least32_t";

    private UIntLeast32TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_least32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedInt };
    }
}