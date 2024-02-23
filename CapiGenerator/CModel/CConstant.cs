using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CConstant(
    Guid compilationUnitId, string name, CConstantExpression constantExpression)
    : BaseCAstItem(compilationUnitId) , ICType
{
    public string Name => name;
    public CConstantExpression ConstantExpression => constantExpression;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in constantExpression.Tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }
}