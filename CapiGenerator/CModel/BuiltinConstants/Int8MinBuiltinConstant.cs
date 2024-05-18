using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class Int8MinBuiltinConstant : BaseBuiltInCConstant
{
    private Int8MinBuiltinConstant() { }

    public static Int8MinBuiltinConstant Instance { get; } = new Int8MinBuiltinConstant();

    public override string Name => "INT8_MIN";

    public override CConstantType GetCConstantType() => CConstantType.Int8_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "INT8_MIN" or "INT_FAST8_MIN" or "INT_LEAST8_MIN" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$INT8_MIN$__";
    }
}
