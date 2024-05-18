using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class Int8MaxBuiltinConstant : BaseBuiltInCConstant
{
    private Int8MaxBuiltinConstant() { }

    public static Int8MaxBuiltinConstant Instance { get; } = new Int8MaxBuiltinConstant();

    public override string Name => "INT8_MAX";

    public override CConstantType GetCConstantType() => CConstantType.Int8_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "INT8_MAX" or "INT_FAST8_MAX" or "INT_LEAST8_MAX" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$INT8_MAX$__";
    }
}
