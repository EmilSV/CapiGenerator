using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class Int64MaxBuiltinConstant : BaseBuiltInCConstant
{
    private Int64MaxBuiltinConstant() { }

    public static Int64MaxBuiltinConstant Instance { get; } = new Int64MaxBuiltinConstant();

    public override string Name => "INT64_MAX";

    public override CConstantType GetCConstantType() => CConstantType.Int64_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "INT64_MAX" or "INT_LEAST64_MAX" or "INT_FAST64_MAX" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$INT64_MAX$__";
    }
}
