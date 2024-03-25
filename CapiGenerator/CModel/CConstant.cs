using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CConstant(
    Guid compilationUnitId, string name, CConstantExpression expression)
    : BaseCAstItem(compilationUnitId), ICConstAssignable
{
    public string Name => name;
    public CConstantExpression Expression => expression;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in expression.Tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }
}