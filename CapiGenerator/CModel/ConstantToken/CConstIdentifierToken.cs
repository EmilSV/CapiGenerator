using CapiGenerator.Parser;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CModel.ConstantToken;

public class CConstIdentifierToken : BaseCConstantToken
{
    private readonly ResoleRef<ICConstAssignable, string> _constantRef;

    public CConstIdentifierToken(string constIdentifierName)
    {
        _constantRef = new(constIdentifierName);
    }

    public CConstIdentifierToken(ICConstAssignable constantIdentifier)
    {
        _constantRef = new(constantIdentifier);
    }

    public ICConstAssignable? GetConstantModel()
    {
        return _constantRef.IsOutputResolved() ? _constantRef.Output : null;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        _constantRef.TrySetOutputFromResolver(compilationUnit);
    }
}