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
            CConstantType.String => new CSConstLiteralToken(token.Value, CSConstantType.String),
            CConstantType.Char => new CSConstLiteralToken(token.Value, CSConstantType.Byte) { CastFromChar = true },
            _ => throw new NotImplementedException()
        };
    }


}