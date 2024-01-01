using CapiGenerator.Model.ConstantToken;
using CapiGenerator.Parser;

namespace CapiGenerator.Model;

public class CConstant(string name, bool isFromEnum, ReadOnlySpan<BaseCConstantToken> tokens) : INeedSecondPass
{
    public readonly string Name = name;
    public readonly bool IsFromEnum = isFromEnum;
    private readonly BaseCConstantToken[] _tokens = tokens.ToArray();

    public string GetConstantIdentifierValue()
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<BaseCConstantToken> GetTokens() => _tokens;
    public ReadOnlySpan<BaseCConstantToken> GetTokensAsSpan() => _tokens;

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in _tokens)
        {
            if (token is INeedSecondPass needSecondPass)
            {
                needSecondPass.OnSecondPass(compilationUnit);
            }
        }
    }
}