using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntFast64tBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntFast64tBuiltinTypedef Instance { get; } = new IntFast64tBuiltinTypedef();

    public override string Name => "int_fast64_t";

    private IntFast64tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_fast64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.LongLong };
    }
}