using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class IntPtrMaxBuiltinConstant : BaseBuiltInCConstant
{
    private IntPtrMaxBuiltinConstant() { }

    public static IntPtrMaxBuiltinConstant Instance { get; } = new IntPtrMaxBuiltinConstant();

    public override string Name => "INTPTR_MAX";

    public override CConstantType GetCConstantType() => CConstantType.IntPtr_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name == "INTPTR_MAX" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$INTPTR_MAX$__";
    }
}
