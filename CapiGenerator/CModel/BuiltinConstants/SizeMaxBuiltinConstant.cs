using CppAst;

namespace CapiGenerator.CModel.BuiltinConstants
{
    public class SizeMaxBuiltinConstant : BaseBuiltInCConstant
    {
        private SizeMaxBuiltinConstant() { }

        public static SizeMaxBuiltinConstant Instance { get; } = new SizeMaxBuiltinConstant();

        public override string Name => "SIZE_MAX";

        public override CConstantType GetCConstantType() => CConstantType.Size_t;

        public override bool MacroIsBuiltin(CppMacro macro)
        {
            return macro.Name == "SIZE_MAX" &&
                macro.Tokens is [{ Kind: CppTokenKind.Identifier } identifierToken] &&
                identifierToken.Text == "__$SIZE_MAX$__";
        }
    }
}