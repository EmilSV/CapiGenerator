namespace CapiGenerator.CSModel.ConstantToken;

public sealed class CSDefaultToken : BaseCSConstantToken
{
    private CSDefaultToken()
    {
    }

    public static CSDefaultToken Instance { get; } = new CSDefaultToken();

    public override string ToString()
    {
        return "default";
    }
}