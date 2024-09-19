using System.Diagnostics.CodeAnalysis;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseBuiltinType : ICSType
{
    public InstanceId Id { get; } = new();

    public bool IsAnonymous => false;

    public abstract string Name { get; }

    public abstract string Namespace { get; }

    public string GetFullName()
    {
        return $"{Namespace}.{Name}";
    }

    public override string ToString()
    {
        return GetFullName();
    }

    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = Name;
        return true;
    }
}