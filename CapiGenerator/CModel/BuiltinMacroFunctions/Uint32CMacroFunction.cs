using System.Globalization;
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
                if (!TryParseUInt32(literalToken.Value, out _))
                {
                    result = null;
                    return false;
                }

                result = new List<BaseCConstantToken>
                {
                    new CConstLiteralToken(literalToken.Value, CConstantType.UInt32_t, literalToken.SourceLocation)
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

        private static bool TryParseUInt32(string value, out uint parsedValue)
        {
            if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                return uint.TryParse(
                    value.AsSpan(2),
                    NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture,
                    out parsedValue);
            }

            return uint.TryParse(
                value,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out parsedValue);
        }
    }
}
