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
        return long.TryParse(Value, NumberStyles.Integer | NumberStyles.HexNumber, null, out value);
    }

    public static CSConstLiteralToken FromCConstantLiteralToken(CConstLiteralToken token)
    {
        return token.Type switch
        {
            CConstantType.Int => new CSConstLiteralToken(token.Value, CSConstantType.Long),
            CConstantType.Float => new CSConstLiteralToken(token.Value, CSConstantType.Double),
            CConstantType.UnsignedInt => new CSConstLiteralToken(token.Value, CSConstantType.Ulong),
            CConstantType.String => new CSConstLiteralToken(token.Value, CSConstantType.Byte) { Utf8Literal = true },
            CConstantType.Char => new CSConstLiteralToken(token.Value, CSConstantType.Byte) { CastFromChar = true },
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