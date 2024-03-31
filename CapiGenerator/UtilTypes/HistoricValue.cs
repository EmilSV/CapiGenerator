namespace CapiGenerator.UtilTypes;

public class HistoricValue<T> : IHistoricChangeNotify<T>
{
    private readonly List<T> _values = [];
    private event Action? TypelessOnChange;

    public HistoricValue(T value)
    {
        _values.Add(value);
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

    public T Value
    {
        get
        {
            if (_values.Count == 0)
            {
                throw new InvalidOperationException("No values in the collection");
            }

            return _values[^1];
        }
    }

    public void SetValue(T value)
    {
        _values.Add(value);
        OnChange?.Invoke(value);
        TypelessOnChange?.Invoke();
    }

    public IReadOnlyList<T> GetHistoricValues()
    {
        return _values;
    }

    public static implicit operator T(HistoricValue<T> historicValues)
    {
        return historicValues.Value;
    }

    public override string? ToString()
    {
        return Value?.ToString();
    }
}
