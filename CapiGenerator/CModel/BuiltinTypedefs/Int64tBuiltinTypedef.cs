using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class Int64tBuiltinTypedef : BaseBuiltinTypedef
{
    public static Int64tBuiltinTypedef Instance { get; } = new Int64tBuiltinTypedef();

    public override string Name => "int64_t";

    private Int64tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.LongLong };
    }
}