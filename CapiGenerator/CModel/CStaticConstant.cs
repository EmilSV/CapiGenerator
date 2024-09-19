using CapiGenerator.CModel.Type;
using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CStaticConstant(
    CTypeInstance realType,
    CConstantType constantType,
    string name,
    CConstantExpression expression)
    : BaseCConstant
{

    public override string Name => name;

    public CTypeInstance RealType => realType;

    public CConstantExpression Expression => expression;

    public override CConstantType GetCConstantType() => constantType;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in expression.Tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }
}