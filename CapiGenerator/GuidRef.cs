using CapiGenerator.Model;

namespace CapiGenerator;

public readonly struct GuidRef<T>
    where T : IGUIDItem
{
    private GuidRef(Guid guid)
    {
        Guid = guid;
    }

    public Guid Guid { get; }

    public override string ToString()
    {
        return Guid.ToString();
    }

    public static implicit operator GuidRef<T>(T item)
    {
        return new(item.Id);
    }
}