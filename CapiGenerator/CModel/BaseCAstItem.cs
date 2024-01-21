using System.Diagnostics.CodeAnalysis;
using CapiGenerator.Parser;

using ObjectType = System.Type;

namespace CapiGenerator.CModel;

public abstract class BaseCAstItem(Guid compilationUnitId)
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


    public readonly Guid CompilationUnitId = compilationUnitId;

    private Dictionary<ObjectType, object>? enrichingData;

    private void AddEnrichingData(ObjectType type, object data)
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

    public void AddEnrichingData(object data)
    {
        if (!reflectionCache.TryGetValue(data.GetType(), out ReflectionCache? cache))
        {
            cache = new ReflectionCache(data.GetType());
        }

        foreach (ObjectType interfaceType in cache.interfaces)
        {
            AddEnrichingData(interfaceType, data);
        }

        foreach (ObjectType interfaceType in cache.baseTypes)
        {
            AddEnrichingData(interfaceType, data);
        }

        AddEnrichingData(cache.selfType, data);
    }

    public int GetEnrichingData<T>(List<T> output)
        where T : class
    {
        if (enrichingData == null)
        {
            return 0;
        }

        if (enrichingData.TryGetValue(typeof(T), out var currentData))
        {
            if (currentData is List<T> currentDataList)
            {
                output.AddRange(currentDataList);
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

    public T? GetEnrichingData<T>()
        where T : class
    {
        if (enrichingData == null)
        {
            return null;
        }

        if (enrichingData.TryGetValue(typeof(T), out var currentData))
        {
            if (currentData is List<T> currentDataList)
            {
                return currentDataList[^1];
            }
            else
            {
                return (T)currentData;
            }
        }

        return null;
    }

    public virtual void OnSecondPass(CCompilationUnit compilationUnit)
    {
    }
}