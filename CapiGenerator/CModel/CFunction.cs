using CapiGenerator.Parser;
using CapiGenerator.Type;

namespace CapiGenerator.CModel;


public class CFunction(
    Guid compilationUnitId, TypeInstance returnType, string name, ReadOnlySpan<CParameter> parameters)
    : BaseCAstItem(compilationUnitId)
{
    private readonly CParameter[] _parameters = parameters.ToArray();
    private TypeInstance _returnType = returnType;

    public string Name => name;
    public TypeInstance ReturnType => _returnType;
    public ReadOnlySpan<CParameter> Parameters => _parameters;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var parameter in Parameters)
        {
            parameter.OnSecondPass(compilationUnit);
        }

        if (_returnType.GetIsCompletedType())
        {
            return;
        }


        _returnType.OnSecondPass(compilationUnit);
    }
}