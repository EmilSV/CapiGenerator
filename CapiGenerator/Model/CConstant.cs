using CapiGenerator.Model.ConstantToken;
using CapiGenerator.Parser;

namespace CapiGenerator.Model;

public class CConstant(Guid compilationUnitId, string name, bool isFromEnum, ReadOnlySpan<BaseCConstantToken> tokens)
    : BaseCAstItem(compilationUnitId)
{
    public readonly string Name = name;
    public readonly bool IsFromEnum = isFromEnum;
    private readonly BaseCConstantToken[] _tokens = tokens.ToArray();
    public ReadOnlySpan<BaseCConstantToken> GetTokens() => _tokens;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in _tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }
}