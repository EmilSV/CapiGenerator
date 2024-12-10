namespace CapiGenerator.CSModel.ConstantToken;

//An escape hatch for arbitrary code
public sealed class CSArbitraryCodeToken(string code) : BaseCSConstantToken
{
    public override string ToString() => code;
}