using CapiGenerator.Model;
using CapiGenerator.Parser;

namespace CapiGenerator.Type;


public class AnonymousFunctionType(
    Guid compilationUnitId, TypeInstance returnType, ReadOnlySpan<CParameter> parameters)
    : BaseAnonymousType(compilationUnitId)
{
    private readonly CParameter[] _parameters = parameters.ToArray();
    private TypeInstance _returnType = returnType;

    public TypeInstance ReturnType => _returnType;
    public ReadOnlySpan<CParameter> Parameters => _parameters;

    public override bool GetIsCompletedType()
    {
        return _returnType.GetIsCompletedType() && _parameters.All(p => p.GetIsCompletedType());
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var parameter in Parameters)
        {
            parameter.OnSecondPass(compilationUnit);
        }

        if (!_returnType.GetIsCompletedType())
        {
            _returnType.OnSecondPass(compilationUnit);
        }
    }
}