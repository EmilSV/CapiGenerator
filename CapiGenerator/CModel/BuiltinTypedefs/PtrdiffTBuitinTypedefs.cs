using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class PtrdiffTBuiltinTypedefs : BaseBuiltinTypedef
{
    public static PtrdiffTBuiltinTypedefs Instance { get; } = new PtrdiffTBuiltinTypedefs();

    public override string Name => "ptrdiff_t";

    private PtrdiffTBuiltinTypedefs()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "ptrdiff_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedLongLong };
    }
}