using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class UInt64MaxBuiltinConstant : BaseBuiltInCConstant
{
    private UInt64MaxBuiltinConstant() { }

    public static UInt64MaxBuiltinConstant Instance { get; } = new UInt64MaxBuiltinConstant();

    public override string Name => "UINT64_MAX";

    public override CConstantType GetCConstantType() => CConstantType.UInt64_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "UINT64_MAX" or "UINT_FAST64_MAX" or "UINT_LEAST64_MAX" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$UINT64_MAX$__";
    }
}
