using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class Int16MinBuiltinConstant : BaseBuiltInCConstant
{
    private Int16MinBuiltinConstant() { }

    public static Int16MinBuiltinConstant Instance { get; } = new Int16MinBuiltinConstant();

    public override string Name => "INT16_MIN";

    public override CConstantType GetCConstantType() => CConstantType.Int16_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "INT16_MIN" or "INT_LEAST16_MIN" or "INT_FAST16_MIN" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$INT16_MIN$__";
    }
}