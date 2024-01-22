using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel;

public sealed class CParameter(
    Guid compilationUnitId, string name, CTypeInstance type) 
    : BaseCAstItem(compilationUnitId)
{
    public readonly string Name = name;
    private CTypeInstance _type = type;
    public CTypeInstance GetParameterType() => _type;

    public bool GetIsCompletedType() => _type.GetIsCompletedType();

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_type.GetIsCompletedType())
        {
            return;
        }
        
        _type.OnSecondPass(compilationUnit);
    }
}