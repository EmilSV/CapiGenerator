using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSUIntPtrType : BaseBuiltinType
{
    public static CSUIntPtrType Instance { get; } = new();

    public override string Name { get; } = nameof(UIntPtr);

    public override string Namespace { get; } = "System";
}