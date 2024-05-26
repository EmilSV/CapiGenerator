using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntFast8TBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntFast8TBuiltinTypedef Instance { get; } = new IntFast8TBuiltinTypedef();

    public override string Name => "int_fast8_t";

    private IntFast8TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int_fast8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Char};
    }
}