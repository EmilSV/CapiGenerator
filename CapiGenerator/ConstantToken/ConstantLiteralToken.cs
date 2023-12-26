using System.Globalization;
namespace CapiGenerator.ConstantToken;

public class ConstantLiteralToken : BaseConstantToken
{
    private readonly ConstantType _type;

    public string Value { get; }
    public ConstantType Type => _type;

    public override string GetOutValue()
    {
        return Type switch
        {
            ConstantType.String => $"\"{Value[1..^1]}\"u8",
            ConstantType.Char => $"(byte){Value}",
            _ => Value,
        };
    }

    public ConstantLiteralToken(string value)
    {
        Value = value;
        _type = GetConstantType(value, out var newValue);
        if (newValue != null)
        {
            Value = newValue;
        }
    }

    public bool TryParseValueAsFloat(out double value)
    {
        return double.TryParse(Value, NumberStyles.Float, null, out value);
    }


    public bool TryParseValueAsInteger(out long value)
    {
        return long.TryParse(Value, NumberStyles.Integer | NumberStyles.HexNumber | NumberStyles., null, out value);
    }

    private static ConstantType GetConstantType(string value, out string? newValue)
    {
        value = value.Trim();
        newValue = value;

        if (value.StartsWith('"') ||
            value.EndsWith('"'))
        {
            return ConstantType.String;
        }

        if (value.StartsWith('\'') ||
            value.EndsWith('\''))
        {
            return ConstantType.Char;
        }

        bool isHex = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
        bool isFloat = value.Contains('.') || value.Contains('e') || value.Contains('E');
        bool isOctal = value.StartsWith("0", StringComparison.OrdinalIgnoreCase) && !isHex;

        if (isHex && long.TryParse(value[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
        {
            return ConstantType.Int;
        }

        if (isFloat && double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
        {
            return ConstantType.Float;
        }

        if (isOctal)
        {
            try
            {
                long octalAsLong = Convert.ToInt64(value[1..], 8);
                newValue = octalAsLong.ToString(CultureInfo.InvariantCulture);
                return ConstantType.Int;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
        {
            return ConstantType.Int;
        }

        return ConstantType.Unknown;
    }
}