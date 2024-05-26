using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntMaxTBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntMaxTBuiltinTypedef Instance { get; } = new IntMaxTBuiltinTypedef();

    public override string Name => "intmax_t";

    private IntMaxTBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "intmax_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.LongLong };
    }
}