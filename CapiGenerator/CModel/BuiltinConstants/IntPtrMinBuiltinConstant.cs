using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class IntPtrMinBuiltinConstant : BaseBuiltInCConstant
{
    private IntPtrMinBuiltinConstant() { }

    public static IntPtrMinBuiltinConstant Instance { get; } = new IntPtrMinBuiltinConstant();

    public override string Name => "INTPTR_MIN";

    public override CConstantType GetCConstantType() => CConstantType.IntPtr_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name == "INTPTR_MIN" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$INTPTR_MIN$__";
    }
}
