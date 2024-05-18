using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class UInt32MaxBuiltinConstant : BaseBuiltInCConstant
{
    private UInt32MaxBuiltinConstant() { }

    public static UInt32MaxBuiltinConstant Instance { get; } = new UInt32MaxBuiltinConstant();

    public override string Name => "UINT32_MAX";

    public override CConstantType GetCConstantType() => CConstantType.UInt32_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "UINT32_MAX" or "UINT_FAST32_MAX" or "UINT_LEAST32_MAX" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$UINT32_MAX$__";
    }
}
