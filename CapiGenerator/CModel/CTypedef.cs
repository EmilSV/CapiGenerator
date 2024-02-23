using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel;

public class CTypedef(Guid compilationUnitId, string name, CTypeInstance innerType) :
    BaseCAstItem(compilationUnitId), ICType
{
    private readonly CTypeInstance _innerType = innerType;
    public string Name => name;
    public CTypeInstance InnerType => _innerType;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_innerType.GetIsCompletedType())
        {
            return;
        }

        _innerType.OnSecondPass(compilationUnit);
    }
}