using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel;


public class CFunction(CTypeInstance returnType, string name, ReadOnlySpan<CParameter> parameters)
    : BaseCAstItem
{
    private readonly CParameter[] _parameters = parameters.ToArray();
    private CTypeInstance _returnType = returnType;

    public string Name => name;
    public CTypeInstance ReturnType => _returnType;
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