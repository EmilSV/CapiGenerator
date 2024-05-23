using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntLeast64tBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntLeast64tBuiltinTypedef Instance { get; } = new IntLeast64tBuiltinTypedef();

    public override string Name => "int_least64_t";

    private IntLeast64tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_least64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.LongLong };
    }
}