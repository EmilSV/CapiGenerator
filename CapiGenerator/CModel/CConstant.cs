using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CConstant(string name, CConstantExpression expression)
    : BaseCConstant
{
    public override string Name => name;

    public CConstantExpression Expression => expression;

    public override CConstantType GetCConstantType()
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