using System.Globalization;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel.BuiltinMacroFunctions;

public class Int64CMacroFunction : BuiltinMacroFunctionBase
{
    public override string Name => "INT64_C";

    public override bool TryEvaluate(
        IReadOnlyList<IReadOnlyList<BaseCConstantToken>> arguments,
        out List<BaseCConstantToken>? result)
    {
        if (arguments is not [var argument] || argument.Count == 0 ||
            argument.OfType<CConstLiteralToken>().Any(literal =>
                literal.Type is CConstantType.Float or CConstantType.Double or CConstantType.String or CConstantType.Char or CConstantType.Unknown))
        {
            result = null;
            return false;
        }

        if (argument is [CConstLiteralToken literalToken])
        {
            if (!TryParseInt64(literalToken.Value, out _))
            {
                result = null;
                return false;
            }

            result =
            [
                new CConstLiteralToken(literalToken.Value, CConstantType.Int64_t, literalToken.SourceLocation)
            ];
            return true;
        }

        var sourceLocation = argument[0].SourceLocation;
        result =
        [
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.LeftParenthesis },
            new CConstCastToken(CPrimitiveType.Instances.LongLong, sourceLocation),
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.LeftParenthesis },
            .. argument,
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis },
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis }
        ];
        return true;
    }

    private static bool TryParseInt64(string value, out long parsedValue)
    {
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            if (ulong.TryParse(
                    value.AsSpan(2),
                    NumberStyles.AllowHexSpecifier,
                    CultureInfo.InvariantCulture,
                    out var unsignedValue) &&
                unsignedValue <= long.MaxValue)
            {
                parsedValue = (long)unsignedValue;
                return true;
            }

            parsedValue = default;
            return false;
        }

        return long.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out parsedValue);
    }
}
