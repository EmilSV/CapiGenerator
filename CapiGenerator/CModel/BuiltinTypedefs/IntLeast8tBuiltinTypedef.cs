using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntLeast8tBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntLeast8tBuiltinTypedef Instance { get; } = new IntLeast8tBuiltinTypedef();

    public override string Name => "int_least8_t";

    private IntLeast8tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_least8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Char};
    }
}