using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSCULongType : ICSType
{
    public static CSCULongType Instance { get; } = new();

    public InstanceId Id { get; } = new();

    public string Namespace { get; } = "System.Runtime.InteropServices";

    public string Name { get; } = "CULong";

    public bool IsAnonymous => false;
}