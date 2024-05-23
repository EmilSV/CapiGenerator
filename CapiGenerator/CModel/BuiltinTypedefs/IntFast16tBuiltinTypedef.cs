using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntFast16tBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntFast16tBuiltinTypedef Instance { get; } = new IntFast16tBuiltinTypedef();

    public override string Name => "int_fast16_t";

    private IntFast16tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_fast16_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Int};
    }
}