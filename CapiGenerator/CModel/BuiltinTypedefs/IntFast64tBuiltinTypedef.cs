using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntFast64TBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntFast64TBuiltinTypedef Instance { get; } = new IntFast64TBuiltinTypedef();

    public override string Name => "int_fast64_t";

    private IntFast64TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_fast64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.LongLong };
    }
}