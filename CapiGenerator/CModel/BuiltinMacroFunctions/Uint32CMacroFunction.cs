using CapiGenerator.CModel.ConstantToken;

namespace CapiGenerator.CModel.BuiltinMacroFunctions
{
    public class Uint32CMacroFunction : BuiltinMacroFunctionBase
    {
        public override string Name => "UINT32_C";

        public override bool TryEvaluate(IReadOnlyList<IReadOnlyList<BaseCConstantToken>> arguments, out List<BaseCConstantToken>? result)
        {
            if (arguments is not [[CConstLiteralToken literalToken]])
            {
                result = null;
                return false;
            }

            if (!uint.TryParse(literalToken.Value, out var value))
            {
                result = null;
                return false;
            }

            result = new List<BaseCConstantToken>
            {
                new CConstLiteralToken(value.ToString(), CConstantType.UInt32_t, literalToken.SourceLocation)
            };

            return true;
        }
    }
}
