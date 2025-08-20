using System.Globalization;
using System.Reflection.Metadata;
namespace CapiGenerator.CModel.ConstantToken;

public class CConstLiteralToken : BaseCConstantToken
{
    private readonly CConstantType _type;

    public string Value { get; }
    public CConstantType Type => _type;

    public CConstLiteralToken(string value)
    {
        Value = value;
        _type = GetConstantType(value, out var newValue);
        if (newValue != null)
        {
            Value = newValue;
        }
    }

    public CConstLiteralToken(string value, CConstantType type)
    {
        Value = value;
        _type = type;
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
        bool isFloat = (value.Contains('.') || value.Contains('e') || value.Contains('E')) && !isHex;
        bool isOctal = value.StartsWith("0", StringComparison.OrdinalIgnoreCase) && !isHex && value.Length > 1;

        bool isUnsigned = false;
        bool isLong = false;
        bool isLongLong = false;
        bool isFloatSuffix = false;
        bool isParsed = false;


        int suffixStart = value.Length - 1;
        for (; suffixStart >= 0; suffixStart--)
        {
            if (value[suffixStart] == 'u' || value[suffixStart] == 'U')
            {
                isUnsigned = true;
            }
            else if (value[suffixStart] == 'l' || value[suffixStart] == 'L')
            {
                if (!isLong && !isLongLong)
                {
                    isLong = true;
                }
                else if (!isLongLong)
                {
                    isLong = false;
                    isLongLong = true;
                }
            }
            else if ((value[suffixStart] == 'f' || value[suffixStart] == 'F') && !isHex)
            {
                isFloatSuffix = true;
            }
            else
            {
                break;
            }
        }
        value = value[..(suffixStart + 1)].ToString();
        newValue = value;



        if (isHex && long.TryParse(value[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _))
        {
            isParsed = true;
        }
        else if (isFloat && double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
        {
            isParsed = true;
        }
        else if (isOctal)
        {
            try
            {
                long octalAsLong = Convert.ToInt64(value[1..], 8);
                newValue = octalAsLong.ToString(CultureInfo.InvariantCulture);
                isParsed = true;
            }
            catch (Exception)
            {
                // ignored
            }
        }
        else if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
        {
            isParsed = true;
        }

        if (!isParsed)
        {
            return CConstantType.Unknown;
        }

        if (isFloat)
        {
            if (isFloatSuffix)
            {
                return CConstantType.Float;
            }
            else
            {
                return CConstantType.Double;
            }
        }

        if (isUnsigned)
        {
            if (isLongLong)
            {
                return CConstantType.UnsignedLongLong;
            }
            return CConstantType.UnsignedInt;
        }
        else
        {
            if (isLongLong)
            {
                return CConstantType.LongLong;
            }
            return CConstantType.Int;
        }
    }
}