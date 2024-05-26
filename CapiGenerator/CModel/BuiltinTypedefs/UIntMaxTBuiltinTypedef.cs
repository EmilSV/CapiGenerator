using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntMaxTBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntMaxTBuiltinTypedef Instance { get; } = new UIntMaxTBuiltinTypedef();

    public override string Name => "uintmax_t";

    private UIntMaxTBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uintmax_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedLongLong };
    }
}