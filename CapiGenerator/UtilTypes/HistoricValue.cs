using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.UtilTypes;

public class HistoricValue<T> : IHistoricChangeNotify<T>
{
    private readonly List<T> _values = [];
    private event Action? TypelessOnChange;

    public HistoricValue()
    {
    }

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
