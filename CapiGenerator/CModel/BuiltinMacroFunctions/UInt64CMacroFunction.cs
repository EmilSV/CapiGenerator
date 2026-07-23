using System.Globalization;
using CapiGenerator.CModel.ConstantToken;

namespace CapiGenerator.CModel.BuiltinMacroFunctions;

public sealed class UInt64CMacroFunction : BuiltinMacroFunctionBase
{
    public override string Name => "UINT64_C";

    public override bool TryEvaluate(
        ReadOnlySpan<BaseCConstantToken> tokens,
        out List<BaseCConstantToken>? result)
    {
        if (tokens is not [CConstLiteralToken literalToken] ||
            !TryParseUInt64(literalToken.Value, out _))
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
