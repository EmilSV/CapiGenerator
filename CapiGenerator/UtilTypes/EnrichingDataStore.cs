using System.Diagnostics.CodeAnalysis;
using ObjectType = System.Type;

namespace CapiGenerator.UtilTypes;

public sealed class EnrichingDataStore
{
    private readonly Dictionary<ObjectType, object> enrichingData = [];
    public void Set<T>(T data)
        where T : notnull
    {
        if (!typeof(T).IsSealed && !typeof(T).IsValueType)
        {
            throw new ArgumentException("T must be a value type or a sealed class");
        }
        if (data is null)
        {
            throw new ArgumentException("data cannot be null");
        }

        enrichingData[typeof(T)] = data;
    }

    public bool SetIfEmpty<T>(T data)
        where T : notnull
    {
        if (!typeof(T).IsSealed && !typeof(T).IsValueType)
        {
            throw new ArgumentException("T must be a value type or a sealed class");
        }
        if (data is null)
        {
            throw new ArgumentException("data cannot be null");
        }

        if (enrichingData.ContainsKey(typeof(T)))
        {
            return false;
        }

        enrichingData[typeof(T)] = data;
        return true;
    }

    public bool Remove<T>()
        where T : notnull
    {
        return enrichingData.Remove(typeof(T));
    }

    public bool TryGetValue<T>([MaybeNullWhen(false)] out T value)
        where T : notnull
    {
        if (enrichingData.TryGetValue(typeof(T), out var untypedValue))
        {
            value = (T)untypedValue;
            return true;
        }
        value = default;
        return false;
    }

    [return: MaybeNull]
    public T Get<T>()
    {
        if (enrichingData.TryGetValue(typeof(T), out var currentData))
        {
            return (T)currentData;
        }

        return default;
    }

    public T Get<T>(Func<T> createIfEmptyFn)
        where T : notnull
    {
        if (enrichingData.TryGetValue(typeof(T), out var currentData))
        {
            return (T)currentData;
        }
        var newData = createIfEmptyFn();
        Set(newData);
        return newData;
    }

    public bool Has<T>()
        where T : notnull
    {
        return enrichingData.ContainsKey(typeof(T));
    }
}