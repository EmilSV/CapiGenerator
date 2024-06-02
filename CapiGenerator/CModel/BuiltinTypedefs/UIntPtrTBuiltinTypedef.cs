using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class UIntPtrTBuiltinTypedef : BaseBuiltinTypedef
{
    public static UIntPtrTBuiltinTypedef Instance { get; } = new UIntPtrTBuiltinTypedef();

    public override string Name => "uintptr_t";

    private UIntPtrTBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "uintptr_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.UnsignedLongLong };
    }
}