using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntLeast16TBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntLeast16TBuiltinTypedef Instance { get; } = new UIntLeast16TBuiltinTypedef();

    public override string Name => "uint_least16_t";

    private UIntLeast16TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uint_least16_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedShort };
    }
}