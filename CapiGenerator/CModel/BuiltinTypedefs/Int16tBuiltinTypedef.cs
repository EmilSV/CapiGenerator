using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class Int16tBuiltinTypedef : BaseBuiltinTypedef
{
    public static Int16tBuiltinTypedef Instance { get; } = new Int16tBuiltinTypedef();

    public override string Name => "int16_t";

    private Int16tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int16_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Short};
    }
}