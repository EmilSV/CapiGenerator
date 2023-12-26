using CapiGenerator.ConstantToken;

namespace CapiGenerator.Model;

public class Constant
{
    private readonly BaseConstantToken[] _tokens;

    public string GetConstantIdentifierValue()
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<BaseConstantToken> GetTokens() => _tokens;
    public ReadOnlySpan<BaseConstantToken> GetTokensAsSpan() => _tokens;

    public Constant(
        ReadOnlySpan<BaseConstantToken> tokens
    )
    {
        _tokens = tokens.ToArray();
    }
}