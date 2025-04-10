using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel;

public sealed class CTypedef(string name, CTypeInstance innerType) :
    BaseCAstItem, ICType
{
    private readonly CTypeInstance _innerType = innerType;
    public string Name => name;
    public CTypeInstance InnerType => _innerType;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        _innerType.OnSecondPass(compilationUnit);
        var type = _innerType.GetCType();
        if (type is not BaseCAnonymousType anonymousType)
        {
            return;
        }

        anonymousType.OnSecondPass(compilationUnit);
    }
}