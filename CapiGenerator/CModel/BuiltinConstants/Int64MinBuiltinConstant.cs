using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class Int64MinBuiltinConstant : BaseBuiltInCConstant
{
    private Int64MinBuiltinConstant() { }

    public static Int64MinBuiltinConstant Instance { get; } = new Int64MinBuiltinConstant();

    public override string Name => "INT64_MIN";

    public override CConstantType GetCConstantType() => CConstantType.Int64_t;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name is "INT64_MIN" or "INT_LEAST64_MIN" or "INT_FAST64_MIN" &&
            macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
            identifierToken.Text == "__$INT64_MIN$__";
    }
}
