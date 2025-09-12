namespace CapiGenerator.CSModel.ConstantToken;


public sealed class CSNullToken : BaseCSConstantToken
{
    private CSNullToken()
    {
    }

    public static CSNullToken Instance { get; } = new CSNullToken();

    public override string ToString()
    {
        return "null";
    }
}