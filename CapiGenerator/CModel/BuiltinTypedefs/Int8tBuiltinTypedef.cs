using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class Int8tBuiltinTypedef : BaseBuiltinTypedef
{
    public static Int8tBuiltinTypedef Instance { get; } = new Int8tBuiltinTypedef();

    public override string Name => "int8_t";

    private Int8tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Char};
    }
}