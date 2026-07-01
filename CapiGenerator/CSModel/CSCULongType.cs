using System.Diagnostics.CodeAnalysis;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSCULongType : BaseBuiltinType
{
    public static CSCULongType Instance { get; } = new();

    public override string Namespace => "System.Runtime.InteropServices";

    public override string Name { get; } = "CULong";
}