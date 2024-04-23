using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CConstant(string name, CConstantExpression expression)
    : BaseCAstItem, ICConstAssignable
{
    public string Name => name;
    public CConstantExpression Expression => expression;

    public CConstantType GetCConstantType()
    {
        return expression.GetTypeOfExpression();
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in expression.Tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }
}