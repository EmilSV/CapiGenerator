using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class UInt8MaxBuiltinConstant : BaseBuiltInCConstant
{
    private UInt8MaxBuiltinConstant() { }

    public static UInt8MaxBuiltinConstant Instance { get; } = new UInt8MaxBuiltinConstant();

    public override string Name => "UINT8_MAX";

    public override CConstantType GetCConstantType() => CConstantType.UInt8_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "UINT8_MAX" or "UINT_FAST8_MAX" or "UINT_LEAST8_MAX" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$UINT8_MAX$__";
    }
}
