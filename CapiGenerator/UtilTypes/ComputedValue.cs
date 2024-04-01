namespace CapiGenerator.UtilTypes;

public class ComputedValue<T> : IHistoricChangeNotify<T>
{
    private readonly Func<T> _compute;
    private Action? TypelessOnChange;
    private T _value;

    public event Action<T>? OnChange;

    public ComputedValue(ReadOnlySpan<IHistoricChangeNotify> dependencies, Func<T> compute)
    {
        _compute = compute;
        _value = _compute();
        foreach (IHistoricChangeNotify dependency in dependencies)
        {
            dependency.OnChange += Recompute;
        }
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

    public T Value
    {
        get
        {
            return _value;
        }
    }

    public void Recompute()
    {
        T newValue = _compute();
        if (newValue == null && _value != null || newValue != null && !newValue.Equals(_value))
        {
            _value = newValue;
            OnChange?.Invoke(newValue);
            TypelessOnChange?.Invoke();
        }
    }

    internal void AddDependency(IHistoricChangeNotify dependency)
    {
        dependency.OnChange += Recompute;
    }


    public static implicit operator T(ComputedValue<T> computedValue)
    {
        return computedValue.Value;
    }

    public override string? ToString()
    {
        return Value?.ToString();
    }
}