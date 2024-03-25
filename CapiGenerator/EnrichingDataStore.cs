using System.Diagnostics.CodeAnalysis;
using ObjectType = System.Type;

namespace CapiGenerator;

public sealed class EnrichingDataStore
{
    private class ReflectionCache
    {
        public List<ObjectType> interfaces;
        public List<ObjectType> baseTypes;
        public ObjectType selfType;

        public ReflectionCache(ObjectType selfType)
        {
            this.selfType = selfType;

            HashSet<ObjectType> interfaces = [];
            HashSet<ObjectType> baseTypes = [];
            AddType(interfaces, baseTypes, selfType);

            this.interfaces = [.. interfaces];
            this.baseTypes = [.. baseTypes];
        }

        private static void AddType(HashSet<ObjectType> interfaces, HashSet<ObjectType> baseTypes, ObjectType? type)
        {
            if (type == null)
            {
                return;
            }

            if (type.IsInterface)
            {
                interfaces.Add(type);
            }
            else
            {
                baseTypes.Add(type);
                AddType(interfaces, baseTypes, type.BaseType);
            }

            foreach (ObjectType interfaceType in type.GetInterfaces())
            {
                AddType(interfaces, baseTypes, interfaceType);
            }
        }
    }

    private readonly static Dictionary<ObjectType, ReflectionCache> reflectionCache = [];

    private Dictionary<ObjectType, object>? enrichingData;

    private void AddOneType(ObjectType type, object data)
    {
        enrichingData ??= [];
        if (enrichingData.TryGetValue(type, out var currentData))
        {
            if (currentData is List<object> currentDataList)
            {
                currentDataList.Add(data);
            }
            else
            {
                enrichingData[type] = (List<object>)[currentData, data];
            }
        }
        else
        {
            enrichingData.Add(type, data);
        }
    }

    public void Add(object data)
    {
        if (!reflectionCache.TryGetValue(data.GetType(), out ReflectionCache? cache))
        {
            cache = new ReflectionCache(data.GetType());
        }

        foreach (ObjectType interfaceType in cache.interfaces)
        {
            AddOneType(interfaceType, data);
        }

        foreach (ObjectType interfaceType in cache.baseTypes)
        {
            AddOneType(interfaceType, data);
        }

        AddOneType(cache.selfType, data);
    }

    public int Get<T>(List<T> output)
    {
        if (enrichingData == null)
        {
            return 0;
        }

        if (enrichingData.TryGetValue(typeof(T), out var currentData))
        {
            if (currentData is List<object> currentDataList)
            {
                foreach (var item in currentDataList)
                {
                    output.Add((T)item);
                }

                return currentDataList.Count;
            }
            else
            {
                output.Add((T)currentData);
                return 1;
            }
        }

        return 0;
    }

    [return: MaybeNull]
    public T Get<T>()
    {
        if (enrichingData == null)
        {
            return default;
        }

        if (enrichingData.TryGetValue(typeof(T), out var currentData))
        {
            if (currentData is List<object> currentDataList)
            {
                return (T)currentDataList[^1];
            }
            else
            {
                return (T)currentData;
            }
        }

        return default;
    }
}