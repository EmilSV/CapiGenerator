using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntFast16TBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntFast16TBuiltinTypedef Instance { get; } = new IntFast16TBuiltinTypedef();

    public override string Name => "int_fast16_t";

    private IntFast16TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_fast16_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Int};
    }
}