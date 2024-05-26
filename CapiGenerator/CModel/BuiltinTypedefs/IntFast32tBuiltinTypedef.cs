using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntFast32TBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntFast32TBuiltinTypedef Instance { get; } = new IntFast32TBuiltinTypedef();

    public override string Name => "int_fast32_t";

    private IntFast32TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_fast32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Int};
    }
}