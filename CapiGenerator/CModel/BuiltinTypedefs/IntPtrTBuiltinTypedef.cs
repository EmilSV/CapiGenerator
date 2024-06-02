using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class IntPtrTBuiltinTypedef : BaseBuiltinTypedef
{
    public static IntPtrTBuiltinTypedef Instance { get; } = new IntPtrTBuiltinTypedef();

    public override string Name => "intptr_t";

    private IntPtrTBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "intptr_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.LongLong };
    }
}