using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class Int8TBuiltinTypedef : BaseBuiltinTypedef
{
    public static Int8TBuiltinTypedef Instance { get; } = new Int8TBuiltinTypedef();

    public override string Name => "int8_t";

    private Int8TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Char };
    }
}