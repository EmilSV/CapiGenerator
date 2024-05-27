using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class SizeTBuiltinTypedefs : BaseBuiltinTypedef
{
    public static SizeTBuiltinTypedefs Instance { get; } = new SizeTBuiltinTypedefs();

    public override string Name => "size_t";

    private SizeTBuiltinTypedefs()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "size_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedLongLong};
    }
}