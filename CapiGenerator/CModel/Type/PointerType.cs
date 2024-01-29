namespace CapiGenerator.CModel.Type;

public sealed class PointerType : CTypeModifier
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
