using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntLeast8tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntLeast8tBuiltinTypedef Instance { get; } = new UIntLeast8tBuiltinTypedef();

    public override string Name => "uint_least8_t";

    private UIntLeast8tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_least8_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedChar};
    }
}