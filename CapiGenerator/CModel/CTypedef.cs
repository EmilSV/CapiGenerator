using CapiGenerator.Parser;
using CapiGenerator.Type;

namespace CapiGenerator.CModel;

public class CTypedef(Guid compilationUnitId, string name, TypeInstance innerType) :
    BaseCAstItem(compilationUnitId), ICType
{
    private readonly TypeInstance _innerType = innerType;
    public string Name => name;
    public TypeInstance InnerType => _innerType;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_innerType.GetIsCompletedType())
        {
            return;
        }

        _innerType.OnSecondPass(compilationUnit);
    }
}