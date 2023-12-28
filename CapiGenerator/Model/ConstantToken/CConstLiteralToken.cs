using System.Globalization;
namespace CapiGenerator.Model.ConstantToken;

public class CConstLiteralToken : BaseCConstantToken
{
    private readonly CConstantType _type;

    public string Value { get; }
    public CConstantType Type => _type;

    // public override string GetOutValue()
    // {
    //     return Type switch
    //     {
    //         ConstantType.String => $"\"{Value[1..^1]}\"u8",
    //         ConstantType.Char => $"(byte){Value}",
    //         _ => Value,
    //     };
    // }

    public CConstLiteralToken(string value)
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
        return long.TryParse(Value, NumberStyles.Integer | NumberStyles.HexNumber, null, out value);
    }

    private static CConstantType GetConstantType(string value, out string? newValue)
    {
        value = value.Trim();
        newValue = value;

        if (value.StartsWith('"') ||
            value.EndsWith('"'))
        {
            return CConstantType.String;
        }

        if (value.StartsWith('\'') ||
            value.EndsWith('\''))
        {
            return CConstantType.Char;
        }

        bool isHex = value.StartsWith("0x", StringComparison.OrdinalIgnoreCase);
        bool isFloat = value.Contains('.') || value.Contains('e') || value.Contains('E');
        bool isOctal = value.StartsWith("0", StringComparison.OrdinalIgnoreCase) && !isHex;

        if (isHex && long.TryParse(value[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
        {
            return CConstantType.Int;
        }

        if (isFloat && double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
        {
            return CConstantType.Float;
        }

        if (isOctal)
        {
            try
            {
                long octalAsLong = Convert.ToInt64(value[1..], 8);
                newValue = octalAsLong.ToString(CultureInfo.InvariantCulture);
                return CConstantType.Int;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
        {
            return CConstantType.Int;
        }

        return CConstantType.Unknown;
    }
}