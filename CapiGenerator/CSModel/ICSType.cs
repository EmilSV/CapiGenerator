using System.Diagnostics.CodeAnalysis;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public interface ICSType
{
    public InstanceId Id { get; }
    public bool IsAnonymous { get; }
    public bool TryGetName([NotNullWhen(true)] out string? name);
}