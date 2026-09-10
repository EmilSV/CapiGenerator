using System.Globalization;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel.BuiltinMacroFunctions;

public sealed class UInt64CMacroFunction : BuiltinMacroFunctionBase
{
    public override string Name => "UINT64_C";

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
            if (!TryParseUInt64(literalToken.Value, out _))
            {
                result = null;
                return false;
            }

            result =
            [
                new CConstLiteralToken(
                    literalToken.Value,
                    CConstantType.UInt64_t,
                    literalToken.SourceLocation)
            ];
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
            new CConstCastToken(CPrimitiveType.Instances.UnsignedLongLong, sourceLocation),
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.LeftParenthesis },
            .. argument,
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis },
            new CConstantPunctuationToken(sourceLocation) { Type = CPunctuationType.RightParenthesis }
        ];
        return true;
    }

    private static bool TryParseUInt64(string value, out ulong parsedValue)
    {
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return ulong.TryParse(
                value.AsSpan(2),
                NumberStyles.AllowHexSpecifier,
                CultureInfo.InvariantCulture,
                out parsedValue);
        }

        return ulong.TryParse(
            value,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out parsedValue);
    }
}
