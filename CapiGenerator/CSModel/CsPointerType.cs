namespace CapiGenerator.CSModel;

public class CsPointerType : CSBaseTypeModifier
{
    public override string GetTypeString()
    {
        return "*";
    }

    private CsPointerType()
    {
    }

    public static CsPointerType Instance { get; } = new();
}