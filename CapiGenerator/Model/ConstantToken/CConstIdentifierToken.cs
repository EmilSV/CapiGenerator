using CapiGenerator.Parser;

namespace CapiGenerator.Model.ConstantToken;

public class CConstIdentifierToken : BaseCConstantToken, INeedSecondPass
{
    private CConstant? _constantModel;
    private readonly string? _constIdentifierName;

    public CConstIdentifierToken(string constIdentifierName)
    {
        _constIdentifierName = constIdentifierName;
    }

    public CConstIdentifierToken(CConstant constantIdentifier)
    {
        _constantModel = constantIdentifier;
    }

    public CConstant? GetConstantModel()
    {
        return _constantModel;
    }

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_constIdentifierName != null)
        {
            _constantModel = compilationUnit.GetConstant(_constIdentifierName);
        }
    }
}