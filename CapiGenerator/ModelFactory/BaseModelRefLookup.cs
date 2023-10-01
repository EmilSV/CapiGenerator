using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator;

public abstract class BaseModelRefLookup<T>
    where T : class, IModel<T>
{
    protected readonly Dictionary<Guid, T> _guidLookup = new();
    protected readonly Dictionary<(string name, string compileUnitNamespace), Guid> _nameGuidLookup = new();
    protected readonly Dictionary<Guid, (string name, string compileUnitNamespace)> _guidNameLookup = new();

    [return: MaybeNull]
    public T Get(ModelRef<T> guidRef)
    {
        if (guidRef.Guid == Guid.Empty)
        {
            return null;
        }

        return _guidLookup.TryGetValue(guidRef.Guid, out var item) ? item : null;
    }

    public ModelRef<T> GetRef(string name, string compileUnitNamespace)
    {
        Guid guid;
        if (_nameGuidLookup.TryGetValue((name, compileUnitNamespace), out guid))
        {
            return new(guid);
        }

        guid = Guid.NewGuid();
        _nameGuidLookup.Add((name, compileUnitNamespace), guid);
        _guidNameLookup.Add(guid, (name, compileUnitNamespace));

        return new(guid);
    }

    public IEnumerable<T> GetValueCollection()
    {
        return _guidLookup.Values;
    }

    public (string? name, string? compileUnitNamespace) GetName(ModelRef<T> guidRef)
    {
        if (guidRef.Guid == Guid.Empty)
        {
            return (null, null);
        }

        return _guidNameLookup.TryGetValue(guidRef.Guid, out var name) ? name : (null, null);
    }
    
    public int RemoveAll(Func<T, bool> predicate)
    {
        var items = _guidLookup.Values.Where(predicate).ToList();
        foreach (var item in items)
        {
            _guidLookup.Remove(item.ModelRef.Guid);
            _nameGuidLookup.Remove((item.InputName, item.CompileUnitNamespace));
            _guidNameLookup.Remove(item.ModelRef.Guid);
        }

        return items.Count;
    }

    internal void TransferOwnership(BaseModelRefLookup<T> other)
    {
        if(this == other)
        {
            return;
        }

        foreach (var item in _guidLookup.Values)
        {   
            item.SetOwingFactory(other);
            other._nameGuidLookup.Add((item.InputName, item.CompileUnitNamespace), item.ModelRef.Guid);
            other._guidNameLookup.Add(item.ModelRef.Guid, (item.InputName, item.CompileUnitNamespace));
            other._guidLookup.Add(item.ModelRef.Guid, item);
        }

        _guidLookup.Clear();
        _nameGuidLookup.Clear();
        _guidNameLookup.Clear();
    }
}