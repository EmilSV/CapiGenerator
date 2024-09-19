using System.Diagnostics.CodeAnalysis;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSUft8LiteralType : ICSType
{
    public static CSUft8LiteralType Instance { get; } = new();

    public InstanceId Id { get; } = new();

    public string Namespace { get; } = "System";

    public string Name { get; } = "ReadonlySpan<byte>";

    public bool IsAnonymous => false;

    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = Name;
        return true;
    }
}