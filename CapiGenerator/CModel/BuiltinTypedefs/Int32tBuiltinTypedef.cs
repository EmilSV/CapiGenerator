using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class Int32tBuiltinTypedef : BaseBuiltinTypedef
{
    public static Int32tBuiltinTypedef Instance { get; } = new Int32tBuiltinTypedef();

    public override string Name => "int32_t";

    private Int32tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Int};
    }
}