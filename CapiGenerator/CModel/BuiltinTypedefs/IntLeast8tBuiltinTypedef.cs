using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntLeast8TBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntLeast8TBuiltinTypedef Instance { get; } = new IntLeast8TBuiltinTypedef();

    public override string Name => "int_least8_t";

    private IntLeast8TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_least8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Char};
    }
}