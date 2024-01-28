using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CConstant(Guid compilationUnitId, string name, bool isFromEnum, ReadOnlySpan<BaseCConstantToken> tokens)
    : BaseCAstItem(compilationUnitId)
{
    public readonly string Name = name;
    public readonly bool IsFromEnum = isFromEnum;
    private readonly BaseCConstantToken[] _tokens = tokens.ToArray();
    public ReadOnlySpan<BaseCConstantToken> GetTokens() => _tokens;
    private CConstantType _constantType = CConstantType.NONE;

    public CConstantType GetConstantType()
    {
        if (_constantType != CConstantType.NONE)
        {
            return _constantType;
        }

        CConstantType constantType = CConstantType.NONE;
        foreach (var token in _tokens)
        {
            if (token is CConstLiteralToken literalToken)
            {
                constantType = literalToken.Type switch
                {
                    CConstantType.Int
                        when _constantType is CConstantType.NONE or CConstantType.Int =>
                            CConstantType.Int,

                    CConstantType.Float or CConstantType.Int
                        when _constantType is CConstantType.NONE or CConstantType.Float or CConstantType.Int =>
                            CConstantType.Float,

                    CConstantType.Char
                        when _constantType is CConstantType.NONE or CConstantType.Char =>
                        CConstantType.Char,

                    CConstantType.String or CConstantType.Char
                        when _constantType is CConstantType.NONE or CConstantType.String or CConstantType.Char =>
                            CConstantType.String,

                    _ => CConstantType.Unknown
                };

            }

            if (constantType is CConstantType.Unknown)
            {
                break;
            }
        }

        _constantType = constantType;

        return _constantType;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in _tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }
}