using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntLeast64tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntLeast64tBuiltinTypedef Instance { get; } = new UIntLeast64tBuiltinTypedef();

    public override string Name => "uint_least64_t";

    private UIntLeast64tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_least64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedLongLong};
    }
}