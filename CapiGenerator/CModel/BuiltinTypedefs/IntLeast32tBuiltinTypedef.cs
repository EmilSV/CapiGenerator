using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntLeast32tBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntLeast32tBuiltinTypedef Instance { get; } = new IntLeast32tBuiltinTypedef();

    public override string Name => "int_least32_t";

    private IntLeast32tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_least32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Int};
    }
}