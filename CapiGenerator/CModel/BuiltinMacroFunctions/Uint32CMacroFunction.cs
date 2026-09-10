using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel.BuiltinMacroFunctions
{
    public class Uint32CMacroFunction : BuiltinMacroFunctionBase
    {
        public override string Name => "UINT32_C";

        public override bool TryEvaluate(
            IReadOnlyList<IReadOnlyList<BaseCConstantToken>> arguments,
            out List<BaseCConstantToken>? result)
        {
            if (arguments is not [var argument] || argument.Count == 0)
            {
                result = null;
                return false;
            }

            if (argument is [CConstLiteralToken literalToken])
            {
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

            if (argument.OfType<CConstLiteralToken>().Any(literal =>
                literal.Type is CConstantType.Float or CConstantType.Double or CConstantType.String or CConstantType.Char or CConstantType.Unknown))
            {
                result = null;
                return false;
            }

            var sourceLocation = argument[0].SourceLocation;
            result =
            [
                new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.LeftParenthesis },
                new CConstCastToken(CPrimitiveType.Instances.UnsignedInt, sourceLocation),
                new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.LeftParenthesis },
                .. argument,
                new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis },
                new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis }
            ];

            return true;
        }
    }
}
