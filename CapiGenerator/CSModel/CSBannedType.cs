using System.Diagnostics.CodeAnalysis;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSBannedType : ICSType
{
    private CSBannedType()
    {
    }

    public static CSBannedType Instance { get; } = new();

    public InstanceId Id { get; } = new();

    public string? Namespace => null;

    public string Name => "__BanedType__";

    public bool IsAnonymous => false;

    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = null;
        return false;
    }
}