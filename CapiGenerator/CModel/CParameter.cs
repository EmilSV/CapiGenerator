using CapiGenerator.Parser;
using CapiGenerator.Type;

namespace CapiGenerator.CModel;

public sealed class CParameter(
    Guid compilationUnitId, string name, TypeInstance type) 
    : BaseCAstItem(compilationUnitId)
{
    public readonly string Name = name;
    private TypeInstance _type = type;
    public TypeInstance GetParameterType() => _type;

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