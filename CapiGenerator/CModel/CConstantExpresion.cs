using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Parser;

namespace CapiGenerator.CModel;


public sealed class CConstantExpression(ReadOnlySpan<BaseCConstantToken> tokens)
{
    private readonly BaseCConstantToken[] _tokens = tokens.ToArray();
    public ReadOnlySpan<BaseCConstantToken> Tokens => _tokens;
    public CConstantType _constantType = CConstantType.NONE;

    public CConstantType GetTypeOfExpression()
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

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in _tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }
}
