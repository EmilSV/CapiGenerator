namespace CapiGenerator.CSModel;

public sealed class CSInlineArrayType(uint size, CSTypeInstance elementType) : BaseBuiltinType
{
    public const uint MinSupportedSize = 1;
    public const uint MaxBuiltInSize = 16;

    public uint Size { get; } = size;

    public CSTypeInstance ElementType { get; } = elementType;

    public override string Namespace => "System.Runtime.CompilerServices";

    public override string Name => $"InlineArray{Size}";

    public override string GetFullName()
    {
        return $"{Namespace}.{Name}<{ElementType}>";
    }
}
