using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel;

namespace CapiGenerator.CSModel;

public class CSEnumValue : BaseCSAstItem
{
    private readonly long? valueNumber;
    private readonly CConstant? valueConstant;

    public string Name { get; }

    public CSEnumValue(string name, long valueNumber)
    {
        Name = name;
        this.valueNumber = valueNumber;
    }

    public CSEnumValue(string name, CConstant valueConstant)
    {
        Name = name;
        this.valueConstant = valueConstant;
    }

    public bool TryGetValueNumber(out long value)
    {
        if (valueNumber.HasValue)
        {
            value = valueNumber.Value;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetValueConstant([NotNullWhen(true)] out CConstant? value)
    {
        if (valueConstant != null)
        {
            value = valueConstant;
            return true;
        }
        value = default;
        return false;
    }
}