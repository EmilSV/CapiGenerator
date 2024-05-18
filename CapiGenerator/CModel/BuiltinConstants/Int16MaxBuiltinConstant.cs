using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class Int16MaxBuiltinConstant : BaseBuiltInCConstant
{
    private Int16MaxBuiltinConstant() { }

    public static Int16MaxBuiltinConstant Instance { get; } = new Int16MaxBuiltinConstant();

    public override string Name => "INT16_MAX";

    public override CConstantType GetCConstantType() => CConstantType.Int16_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "INT16_MAX" or "INT_FAST16_MAX" or "INT_LEAST16_MAX" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$INT16_MAX$__";
    }
}
