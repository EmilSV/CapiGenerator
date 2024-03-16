using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;
using CppAst;

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

    public static CParameter FromCPPParameter(Guid compilationUnitId, CppParameter parameter)
    {
        var type = TypeConverter.PartialConvert(compilationUnitId, parameter.Type);
        return new CParameter(compilationUnitId, parameter.Name, type);
    }
}