using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public interface ICSType
{
    public InstanceId Id { get; }
    public string? Namespace { get; }
    public string Name { get; }

    public string GetFullName() => Namespace != null ? $"{Namespace}.{Name}" : Name;
}