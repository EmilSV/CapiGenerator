using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator;

public readonly struct GuidRef<T>
    where T : class, IGUIDItem<T>
{
    private GuidRef(Guid guid, LookupCollection lookup)
    {
        Guid = guid;
        Lookup = lookup;
    }

    public readonly Guid Guid { get; }
    public readonly LookupCollection Lookup { get; }

    public override string ToString()
    {
        return Guid.ToString();
    }

    public bool IsValid()
    {
        return Guid != Guid.Empty;
    }

    [return: MaybeNull]
    public T Get()
    {
        return Lookup.Get(this);
    }

    public (string? name, string? compileUnitNamespace) GetNameInfo()
    {
        return Lookup.GetName(this);
    }

    public static implicit operator GuidRef<T>(T item)
    {
        return item.Id;
    }

    public sealed class LookupCollection
    {
        private readonly Dictionary<Guid, T> _guidLookup = new();
        private readonly Dictionary<(string name, string compileUnitNamespace), Guid> _nameLookup = new();
        private readonly Dictionary<Guid, (string name, string compileUnitNamespace)> _guidNameLookup = new();

        [return: MaybeNull]
        public T Get(GuidRef<T> guidRef)
        {
            if (guidRef.Guid == Guid.Empty)
            {
                return null;
            }

            return _guidLookup.TryGetValue(guidRef.Guid, out var item) ? item : null;
        }

        public GuidRef<T> Get(string name, string compileUnitNamespace)
        {
            Guid guid;
            if (_nameLookup.TryGetValue((name, compileUnitNamespace), out guid))
            {
                return new(guid, this);
            }

            guid = Guid.NewGuid();
            _nameLookup.Add((name, compileUnitNamespace), guid);
            _guidNameLookup.Add(guid, (name, compileUnitNamespace));

            return new(guid, this);
        }

        public (string? name, string? compileUnitNamespace) GetName(GuidRef<T> guidRef)
        {
            if (guidRef.Guid == Guid.Empty)
            {
                return (null, null);
            }

            return _guidNameLookup.TryGetValue(guidRef.Guid, out var name) ? name : (null, null);
        }

        public void Add(T item)
        {
            _guidLookup.Add(item.Id.Guid, item);
        }

        public Dictionary<Guid, T>.ValueCollection GetValueCollection()
        {
            return _guidLookup.Values;
        }
    }
}