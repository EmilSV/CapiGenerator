using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSCLongType : ICSType
{
    public static CSCLongType Instance { get; } = new();

    public InstanceId Id { get; } = new();

    public string Namespace { get; } = "System.Runtime.InteropServices";

    public string Name { get; } = "CLong";

    public bool IsAnonymous => false;
}