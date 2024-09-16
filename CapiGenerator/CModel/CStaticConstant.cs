using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CStaticConstant(CConstantType type, string name, CConstantExpression expression)
    : BaseCConstant
{

    public override string Name => name;

    public CConstantExpression Expression => expression;

    public override CConstantType GetCConstantType() => type;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in expression.Tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }
}