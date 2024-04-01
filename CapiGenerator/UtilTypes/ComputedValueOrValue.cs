using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.UtilTypes;

public readonly struct ComputedValueOrValue<T>
{
    private readonly ComputedValue<T>? _computedValue;
    private readonly T _value;

    public ComputedValueOrValue(T value)
    {
        _value = value;
        _computedValue = null;
    }

    public ComputedValueOrValue(ComputedValue<T> computedValue)
    {
        _value = computedValue.Value;
        _computedValue = computedValue;
    }

    public T Value => _computedValue == null ? _value : _computedValue.Value;

    public bool TryAsComputedValue([NotNullWhen(true)] out ComputedValue<T>? computedValue)
    {
        computedValue = _computedValue;
        return _computedValue != null;
    }

    public bool TryAsValue([NotNullWhen(true)][MaybeNull] out T value)
    {
        value = _value;
        return _computedValue != null;
    }


    public static implicit operator ComputedValueOrValue<T>(T value) => new(value);
    public static implicit operator ComputedValueOrValue<T>(HistoricValue<T> value) => new(value);
    public static implicit operator ComputedValueOrValue<T>(ComputedValue<T> computedValue) => new(computedValue);
    public static implicit operator T(ComputedValueOrValue<T> computedValueOrValue) => computedValueOrValue.Value;
}