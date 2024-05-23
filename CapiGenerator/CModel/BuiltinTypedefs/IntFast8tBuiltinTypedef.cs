using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntFast8tBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntFast8tBuiltinTypedef Instance { get; } = new IntFast8tBuiltinTypedef();

    public override string Name => "int_fast8_t";

    private IntFast8tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_fast8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Char};
    }
}