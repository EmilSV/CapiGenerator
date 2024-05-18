using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class UInt16MaxBuiltinConstant : BaseBuiltInCConstant
{
    private UInt16MaxBuiltinConstant() { }

    public static UInt16MaxBuiltinConstant Instance { get; } = new UInt16MaxBuiltinConstant();

    public override string Name => "UINT16_MAX";

    public override CConstantType GetCConstantType() => CConstantType.UInt16_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "UINT16_MAX" or "UINT_FAST16_MAX" or "UINT_LEAST16_MAX" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$UINT16_MAX$__";
    }
}
