using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;
using CppAst;

namespace CapiGenerator.CModel;

public sealed class CParameter(string name, CTypeInstance type)
    : BaseCAstItem
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

    public static CParameter FromCPPParameter(CppParameter parameter)
    {
        var type = CTypeInstance.FromCppType(parameter.Type);
        return new CParameter(parameter.Name, type);
    }
}