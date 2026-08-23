using System.Globalization;
using System.Text;
using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
namespace CapiGenerator.CSModel.ConstantToken;

public class CSConstLiteralToken(string value, CSConstantType type) : BaseCSConstantToken
{
    public string Value = value;
    public CSConstantType Type => type;
    public bool CastFromChar { get; init; } = false;
    public bool Utf8Literal { get; init; } = false;

    public bool TryParseValueAsFloat(out double value)
    {
        return double.TryParse(Value, NumberStyles.Float, null, out value);
    }

    public bool TryParseValueAsInteger(out long value)
    {
        var valueToParse = Value.Trim();
        if (valueToParse.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return long.TryParse(
                valueToParse[2..],
                NumberStyles.AllowHexSpecifier,
                CultureInfo.InvariantCulture,
                out value);
        }

        return long.TryParse(valueToParse, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
    }

    public static CSConstLiteralToken FromCConstantLiteralToken(CConstLiteralToken token)
    {
        return token.Type switch
        {
            CConstantType.Int or CConstantType.Int32_t => new CSConstLiteralToken(token.Value, CSConstantType.Int),
            CConstantType.UnsignedInt or CConstantType.UInt32_t => new CSConstLiteralToken(token.Value, CSConstantType.Uint),
            CConstantType.LongLong or CConstantType.Int64_t => new CSConstLiteralToken(token.Value, CSConstantType.Long),
            CConstantType.UnsignedLongLong or CConstantType.UInt64_t => new CSConstLiteralToken(token.Value, CSConstantType.Ulong),
            CConstantType.Float => new CSConstLiteralToken(token.Value, CSConstantType.Float),
            CConstantType.Double => new CSConstLiteralToken(token.Value, CSConstantType.Double),
            CConstantType.String => new CSConstLiteralToken(token.Value, CSConstantType.Byte) { Utf8Literal = true },
            CConstantType.Char or CConstantType.UInt8_t => new CSConstLiteralToken(token.Value, CSConstantType.Byte) { CastFromChar = true },
            _ => throw new NotImplementedException()
        };
    }

    public override string ToString() => type switch
    {
        CSConstantType.Byte when CastFromChar == true => $"(byte)'{Value}'",
        CSConstantType.Byte when Utf8Literal == true => Value,
        _ => Value
    };
}