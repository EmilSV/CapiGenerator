using System.Diagnostics.CodeAnalysis;
using CapiGenerator.Parser;

using ObjectType = System.Type;

namespace CapiGenerator.Model;

public abstract class BaseCAstItem(Guid compilationUnitId)
{
    public readonly Guid CompilationUnitId = compilationUnitId;

    private Dictionary<ObjectType, object>? enrichingData;

    public void AddEnrichingData<T>(T data)
        where T : class
    {
        enrichingData ??= [];
        if (enrichingData.TryGetValue(typeof(T), out var currentData))
        {
            if (currentData is List<T> currentDataList)
            {
                currentDataList.Add(data);
            }
            else
            {
                enrichingData[typeof(T)] = (List<T>)[(T)currentData, data];
            }
        }
        else
        {
            enrichingData.Add(typeof(T), data);
        }

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