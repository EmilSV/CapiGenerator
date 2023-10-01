using System.Globalization;
using CapiGenerator.Model;
namespace CapiGenerator.ConstantToken;

public record class ConstantLiteralToken : BaseConstantToken
{
    private ConstantType _type;

    public string Value { get; }
    public ConstantType Type => _type;

    public override string GetOutValue(BaseModelRefLookup<Constant> constLookup)
    {
        switch (Type)
        {
            case ConstantType.String:
                return $"\"{Value[1..^1]}\"u8";
            case ConstantType.Char:
                return $"(byte){Value}";
        }

        return Value;
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

    public static ConstantType GetConstantType(string value, out string? newValue)
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