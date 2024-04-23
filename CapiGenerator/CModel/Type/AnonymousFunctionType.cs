using CapiGenerator.Parser;
using CppAst;

namespace CapiGenerator.CModel.Type;


public class AnonymousFunctionType(CTypeInstance returnType, ReadOnlySpan<CParameter> parameters)
    : BaseCAnonymousType
{
    private readonly CParameter[] _parameters = parameters.ToArray();
    public CTypeInstance ReturnType => returnType;
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
        if (GetIsCompletedType())
        {
            return;
        }

        foreach (var parameter in Parameters)
        {
            parameter.OnSecondPass(compilationUnit);
        }
        returnType.OnSecondPass(compilationUnit);
    }

    public static AnonymousFunctionType FromCFunctionType(CppFunctionType function)
    {
        var returnType = CTypeInstance.FromCppType(function.ReturnType);
        var parameters = function.Parameters.Select(CParameter.FromCPPParameter).ToArray();
        return new AnonymousFunctionType(returnType, parameters);
    }
}