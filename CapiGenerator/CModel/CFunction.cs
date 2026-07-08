using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;
using CppAst;
using CapiGenerator.CModel.Comments;

namespace CapiGenerator.CModel;


public class CFunction(CTypeInstance returnType, string name, ReadOnlySpan<CParameter> parameters)
    : BaseCAstItem
{
    private readonly CParameter[] _parameters = parameters.ToArray();
    private readonly CTypeInstance _returnType = returnType;

    public string Name => name;
    public CTypeInstance ReturnType => _returnType;
    public ReadOnlySpan<CParameter> Parameters => _parameters;
    public CBaseComment? Comment { get; init; }

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

    public static CFunction? From(CppFunction function)
    {
        var parameters = function.Parameters.Select(CParameter.From).ToArray();
        if (parameters == null || parameters.Any(parameter => parameter is null))
        {
            return null;
        }

        var returnType = CTypeInstance.FromCppType(function.ReturnType);

        return new CFunction(returnType, function.Name, parameters!)
        {
            Comment = CBaseComment.From(function.Comment),
        };
    }
}
