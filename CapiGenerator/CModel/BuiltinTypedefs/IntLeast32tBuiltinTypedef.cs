using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntLeast32TBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntLeast32TBuiltinTypedef Instance { get; } = new IntLeast32TBuiltinTypedef();

    public override string Name => "int_least32_t";

    private IntLeast32TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_least32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Int };
    }
}