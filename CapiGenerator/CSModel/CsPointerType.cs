namespace CapiGenerator.CSModel;

public class CsPointerType : BaseCSTypeModifier
{
    public override string GetTypePostFixString()
    {
        return "*";
    }

    private CsPointerType()
    {
    }

    public static CsPointerType Instance { get; } = new();
}
