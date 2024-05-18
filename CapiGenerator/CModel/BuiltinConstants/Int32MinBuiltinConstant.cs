using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class Int32MinBuiltinConstant : BaseBuiltInCConstant
{
    private Int32MinBuiltinConstant() { }

    public static Int32MinBuiltinConstant Instance { get; } = new Int32MinBuiltinConstant();

    public override string Name => "INT32_MIN";

    public override CConstantType GetCConstantType() => CConstantType.Int32_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "INT32_MIN" or "INT_LEAST32_MIN" or "INT_FAST32_MIN" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$INT32_MIN$__";
    }
}