using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel;

namespace CapiGenerator.CSModel;


public readonly struct CSDefaultValue
{
    private readonly object? _value;
    public readonly object? Value => _value;

    public CSDefaultValue(CSConstantExpression value)
    {
        _value = value;
    }

    public CSDefaultValue(string value)
    {
        _value = value;
    }

    public CSDefaultValue(double value)
    {
        _value = value;
    }

    public CSDefaultValue(ulong value)
    {
        _value = value;
    }

    public CSDefaultValue(bool value)
    {
        _value = value;
    }


    public bool TryGetDouble(out double value)
    {
        if (_value is double d)
        {
            value = d;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetString([MaybeNullWhen(false)] out string value)
    {
        if (_value is string s)
        {
            value = s;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetBool(out bool value)
    {
        if (_value is bool b)
        {
            value = b;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetUlong(out ulong value)
    {
        if (_value is ulong u)
        {
            value = u;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetCSConstantExpression([MaybeNullWhen(false)]  out CSConstantExpression value)
    {
        if (_value is CSConstantExpression c)
        {
            value = c;
            return true;
        }

        value = default;
        return false;
    }
}