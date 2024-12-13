using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants;

public class NanBuiltinConstant : BaseBuiltInCConstant
{
    private NanBuiltinConstant() { }

    public static NanBuiltinConstant Instance { get; } = new NanBuiltinConstant();

    public override string Name => "NAN";

    public override CConstantType GetCConstantType() => CConstantType.Float;

    public override bool MacroIsBuiltin(CppMacro macro)
    {
        return macro.Name == "NAN" &&
        macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
        identifierToken.Text == "__$NAN$__";
    }
}