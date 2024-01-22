using CapiGenerator.CModel;

namespace CapiGenerator.CSModel;


public readonly struct CSDefaultValue
{
    private readonly object? _value;
    public readonly object? Value => _value;

    public CSDefaultValue(CConstant value)
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
}