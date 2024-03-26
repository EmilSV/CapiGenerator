namespace CapiGenerator.Collection;

public class HistoricValues<T>
{
    public HistoricValues(T value)
    {
        _values.Add(value);
    }

    private List<T> _values { get; set; } = new();

    public T Value
    {
        get
        {
            if (_values.Count == 0)
            {
                throw new InvalidOperationException("No values in the collection");
            }

            return _values[_values.Count - 1];
        }
    }

    public void SetValue(T value)
    {
        _values.Add(value);
    }

    public IReadOnlyList<T> GetHistoricValues()
    {
        return _values;
    }

    public static implicit operator T(HistoricValues<T> historicValues)
    {
        return historicValues.Value;
    }

    public override string? ToString()
    {
        return Value?.ToString();
    }    
}
