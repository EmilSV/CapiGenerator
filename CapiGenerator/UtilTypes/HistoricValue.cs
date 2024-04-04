using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.UtilTypes;

public class HistoricValue<T> : IHistoricChangeNotify<T>
{
    private readonly List<T> _values = [];
    private readonly bool _isReadOnly;
    private event Action? TypelessOnChange;

    private HistoricValue(bool isReadOnly)
    {
        _isReadOnly = isReadOnly;
    }

    public HistoricValue()
    {
    }

    public HistoricValue(T value)
    {
        _values.Add(value);
    }

    public static HistoricValue<T> NewReadOnly(T value)
    {
        var historicValue = new HistoricValue<T>(isReadOnly: true);
        historicValue._values.Add(value);
        return historicValue;
    }


    event Action? IHistoricChangeNotify.OnChange
    {
        add
        {
            TypelessOnChange += value;
        }

        remove
        {
            TypelessOnChange -= value;
        }
    }

    public event Action<T>? OnChange;

    public bool IsReadOnly => _isReadOnly;

    [MaybeNull]
    public T Value
    {
        get
        {
            if (_values.Count == 0)
            {
                return default;
            }

            return _values[^1];
        }
    }

    public void SetValue(T value)
    {
        if (_isReadOnly)
        {
            throw new InvalidOperationException("Cannot set value on readonly HistoricValue");
        }

        _values.Add(value);
        OnChange?.Invoke(value);
        TypelessOnChange?.Invoke();
    }

    public IReadOnlyList<T> GetHistoricValues()
    {
        return _values;
    }

    [return: MaybeNull]
    public static implicit operator T(HistoricValue<T> historicValues)
    {
        return historicValues.Value;
    }

    public override string? ToString()
    {
        return Value?.ToString();
    }
}
