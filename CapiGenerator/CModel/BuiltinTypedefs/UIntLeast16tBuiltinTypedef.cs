using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntLeast16tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntLeast16tBuiltinTypedef Instance { get; } = new UIntLeast16tBuiltinTypedef();

    public override string Name => "uint_least16_t";

    private UIntLeast16tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_least16_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedShort };
    }
}