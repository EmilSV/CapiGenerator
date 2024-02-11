using System.Globalization;
namespace CapiGenerator.CSModel.ConstantToken;

public class CSConstLiteralToken(string value, CSConstantType type) : BaseCSConstantToken
{
    public string Value = value;
    public CSConstantType Type => type;

    public bool TryParseValueAsFloat(out double value)
    {
        return double.TryParse(Value, NumberStyles.Float, null, out value);
    }


    public bool TryParseValueAsInteger(out long value)
    {
        return long.TryParse(Value, NumberStyles.Integer | NumberStyles.HexNumber, null, out value);
    }
}