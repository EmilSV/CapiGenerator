using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntLeast8TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntLeast8TBuiltinTypedef Instance { get; } = new UIntLeast8TBuiltinTypedef();

    public override string Name => "uint_least8_t";

    private UIntLeast8TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_least8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedChar};
    }
}