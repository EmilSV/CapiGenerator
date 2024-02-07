using CapiGenerator.Parser;

namespace CapiGenerator.CSModel.ConstantToken;

public class CConstIdentifierToken : BaseCSConstantToken
{
    private CSField? _constantField;
    private readonly string? _constIdentifierName;

    public CConstIdentifierToken(string constIdentifierName)
    {
        _constIdentifierName = constIdentifierName;
    }

    public CConstIdentifierToken(CSField constantField)
    {
        _constantField = constantField;
    }

    public CSField? GetField()
    {
        return _constantField;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_constIdentifierName != null)
        {
            _constantModel = compilationUnit.GetConstantByName(_constIdentifierName);
        }
    }
}