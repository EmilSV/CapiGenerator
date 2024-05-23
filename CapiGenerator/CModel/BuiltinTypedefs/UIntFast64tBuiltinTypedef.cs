using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntFast64tBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntFast64tBuiltinTypedef Instance { get; } = new UIntFast64tBuiltinTypedef();

    public override string Name => "uint_fast64_t";

    private UIntFast64tBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_fast64_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedLongLong};
    }
}