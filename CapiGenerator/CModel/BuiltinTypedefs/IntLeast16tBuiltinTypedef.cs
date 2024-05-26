using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntLeast16TBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntLeast16TBuiltinTypedef Instance { get; } = new IntLeast16TBuiltinTypedef();

    public override string Name => "int_least16_t";

    private IntLeast16TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_least16_t" && 
        typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Short};
    }
}