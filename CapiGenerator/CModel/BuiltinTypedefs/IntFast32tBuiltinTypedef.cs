using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntFast32tBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntFast32tBuiltinTypedef Instance { get; } = new IntFast32tBuiltinTypedef();

    public override string Name => "int_fast32_t";

    private IntFast32tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_fast32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Int};
    }
}