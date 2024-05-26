using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class Int64TBuiltinTypedef : BaseBuiltinTypedef
{
    public static Int64TBuiltinTypedef Instance { get; } = new Int64TBuiltinTypedef();

    public override string Name => "int64_t";

    private Int64TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.LongLong };
    }
}