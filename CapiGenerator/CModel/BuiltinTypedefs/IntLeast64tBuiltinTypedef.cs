using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntLeast64TBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntLeast64TBuiltinTypedef Instance { get; } = new IntLeast64TBuiltinTypedef();

    public override string Name => "int_least64_t";

    private IntLeast64TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_least64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.LongLong };
    }
}