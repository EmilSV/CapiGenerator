using CapiGenerator.Parser;

namespace CapiGenerator.CModel.ConstantToken;

public class CConstIdentifierToken : BaseCConstantToken
{
    private readonly ResoleRef<CConstant, string> _constantRef;

    public CConstIdentifierToken(string constIdentifierName)
    {
        _constantRef = new(constIdentifierName);
    }

    public CConstIdentifierToken(CConstant constantIdentifier)
    {
        _constantRef = new(constantIdentifier);
    }

    public CConstant? GetConstantModel()
    {
        return _constantRef.IsOutputResolved() ? _constantRef.Output : null;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        _constantRef.TrySetOutputFromResolver(compilationUnit);
    }
}