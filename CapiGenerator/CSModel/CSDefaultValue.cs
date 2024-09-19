using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel;
using CapiGenerator.CSModel.ConstantToken;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;


public readonly record struct CSDefaultValue
{
    public readonly object? Value { get; }

    public CSDefaultValue(CSConstantExpression value)
    {
        Value = value;
    }

    public CSDefaultValue(string value)
    {
        Value = value;
    }

    public CSDefaultValue(double value)
    {
        Value = value;
    }

    public CSDefaultValue(ulong value)
    {
        Value = value;
    }

    public CSDefaultValue(long value)
    {
        Value = value;
    }

    public CSDefaultValue(bool value)
    {
        Value = value;
    }


    public bool TryGetDouble(out double value)
    {
        if (Value is double d)
        {
            value = d;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetString([MaybeNullWhen(false)] out string value)
    {
        if (Value is string s)
        {
            value = s;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetBool(out bool value)
    {
        if (Value is bool b)
        {
            value = b;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetUlong(out ulong value)
    {
        if (Value is ulong u)
        {
            value = u;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetLong(out long value)
    {
        if (Value is long u)
        {
            value = u;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetCSConstantExpression([MaybeNullWhen(false)] out CSConstantExpression value)
    {
        if (Value is CSConstantExpression c)
        {
            value = c;
            return true;
        }

        value = default;
        return false;
    }

    public void OnSecondPass(CSTranslationUnit unit)
    {
        if (Value is CSConstantExpression c)
        {
            c.OnSecondPass(unit);
        }
    }


    public CSConstantExpression? ToCSConstantExpression()
    {
        return Value switch
        {
            CSConstantExpression c => c,
            long l => [new CSConstLiteralToken(l.ToString(), CSConstantType.Long)],
            ulong ul => [new CSConstLiteralToken(ul.ToString(), CSConstantType.Ulong)],
            double d => [new CSConstLiteralToken(d.ToString(), CSConstantType.Double)],
            string s => [new CSConstLiteralToken(s, CSConstantType.String)],
            bool b => [new CSConstLiteralToken(b.ToString(), CSConstantType.Bool)],
            _ => null
        };
    }

    public static readonly CSDefaultValue NullValue = default;
    public static readonly CSDefaultValue DefaultValue = new(new CSConstantExpression([CSDefaultToken.Instance]));
}
