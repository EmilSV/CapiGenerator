using CapiGenerator.CModel;
using CapiGenerator.Parser;

namespace CapiGenerator.Type;


public class AnonymousFunctionType(
    Guid compilationUnitId, TypeInstance returnType, ReadOnlySpan<CParameter> parameters)
    : BaseAnonymousType(compilationUnitId)
{
    private readonly CParameter[] _parameters = parameters.ToArray();
    public TypeInstance ReturnType => returnType;
    public ReadOnlySpan<CParameter> Parameters => _parameters;

    private bool _isCompletedType = false;

    public override bool GetIsCompletedType()
    {
        if (_isCompletedType)
        {
            return true;
        }

        _isCompletedType = returnType.GetIsCompletedType() && _parameters.All(p => p.GetIsCompletedType());
        return _isCompletedType;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if(GetIsCompletedType())
        {
            return;
        }

        foreach (var parameter in Parameters)
        {
            parameter.OnSecondPass(compilationUnit);
        }
        returnType.OnSecondPass(compilationUnit);
    }
}