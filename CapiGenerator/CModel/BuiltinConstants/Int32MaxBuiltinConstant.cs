using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class Int32MaxBuiltinConstant : BaseBuiltInCConstant
{
    private Int32MaxBuiltinConstant() { }

    public static Int32MaxBuiltinConstant Instance { get; } = new Int32MaxBuiltinConstant();

    public override string Name => "INT32_MAX";

    public override CConstantType GetCConstantType() => CConstantType.Int32_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "INT32_MAX" or "INT_LEAST32_MAX" or "INT_FAST32_MAX" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$INT32_MAX$__";
    }
}