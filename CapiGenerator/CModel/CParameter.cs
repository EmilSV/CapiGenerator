using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;
using CppAst;
using CapiGenerator.CModel.Comments;

namespace CapiGenerator.CModel;

public sealed class CParameter(string name, CTypeInstance type)
    : BaseCAstItem
{
    public readonly string Name = name;
    private CTypeInstance _type = type;
    public CTypeInstance GetParameterType() => _type;

    public CBaseComment? Comment { get; init; }

    public bool GetIsCompletedType() => _type.GetIsCompletedType();

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_type.GetIsCompletedType())
        {
            return;
        }

        _type.OnSecondPass(compilationUnit);
    }

    public static CParameter From(CppParameter parameter)
    {
        var type = CTypeInstance.FromCppType(parameter.Type);
        return new CParameter(parameter.Name, type)
        {
            Comment = CBaseComment.From(parameter.Comment)
        };
    }
}
