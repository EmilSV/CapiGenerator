using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntLeast16tBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntLeast16tBuiltinTypedef Instance { get; } = new IntLeast16tBuiltinTypedef();

    public override string Name => "int_least16_t";

    private IntLeast16tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_least16_t" && 
        typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Short};
    }
}