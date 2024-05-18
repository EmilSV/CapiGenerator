using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class UIntPtrMaxBuiltinConstant : BaseBuiltInCConstant
{
    private UIntPtrMaxBuiltinConstant() { }

    public static UIntPtrMaxBuiltinConstant Instance { get; } = new UIntPtrMaxBuiltinConstant();

    public override string Name => "UINTPTR_MAX";

    public override CConstantType GetCConstantType() => CConstantType.UIntPtr_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name == "UINTPTR_MAX" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$UINTPTR_MAX$__";
    }
}
