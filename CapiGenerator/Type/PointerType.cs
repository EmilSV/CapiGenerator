namespace CapiGenerator.Type;

public sealed class PointerType : TypeModifier
{
    public override string GetTypeString()
    {
        return "*";
    }

    private PointerType()
    {
    }

    public static PointerType Instance { get; } = new();
}
