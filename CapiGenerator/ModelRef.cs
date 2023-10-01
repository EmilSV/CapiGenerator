using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator;

public readonly struct ModelRef<T>
    where T : class, IModel<T>
{
    internal ModelRef(Guid guid)
    {
        Guid = guid;
    }

    public readonly Guid Guid { get; }

    public override string ToString()
    {
        return Guid.ToString();
    }

    public bool IsValid()
    {
        return Guid != Guid.Empty;
    }

    [return: MaybeNull]
    public T Get(BaseModelRefLookup<T> factory)
    {
        return factory.Get(this);
    }

    public (string? name, string? compileUnitNamespace) GetNameInfo(BaseModelRefLookup<T> factory)
    {
        return factory.GetName(this);
    }

    public static implicit operator ModelRef<T>(T item)
    {
        return item.ModelRef;
    }
}