using CapiGenerator.Parser;

namespace CapiGenerator.CModel.ConstantToken;

public class CConstIdentifierToken : BaseCConstantToken
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

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_constIdentifierName != null)
        {
            _constantModel = compilationUnit.GetConstantByName(_constIdentifierName);
        }
    }
}