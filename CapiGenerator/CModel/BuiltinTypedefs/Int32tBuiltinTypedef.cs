using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class Int32TBuiltinTypedef : BaseBuiltinTypedef
{
    public static Int32TBuiltinTypedef Instance { get; } = new Int32TBuiltinTypedef();

    public override string Name => "int32_t";

    private Int32TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int32_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Int};
    }
}