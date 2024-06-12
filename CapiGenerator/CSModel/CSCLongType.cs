using System.Runtime.InteropServices;

namespace CapiGenerator.CSModel;

public sealed class CSCLongType : BaseBuiltinType
{
    public static CSCLongType Instance { get; } = new();

    public override string Namespace { get; } = "System.Runtime.InteropServices";

    public override string Name { get; } = nameof(CLong);
}