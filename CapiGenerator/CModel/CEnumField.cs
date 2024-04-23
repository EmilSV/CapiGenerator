using CapiGenerator.Parser;

namespace CapiGenerator.CModel;


public sealed class CEnumField(string name, CConstantExpression expression) :
    BaseCAstItem, ICConstAssignable
{
    public string Name => name;
    public CConstantExpression Expression => expression;

    public CConstantType GetCConstantType()
    {
        return CConstantType.Int;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        expression.OnSecondPass(compilationUnit);
    }
}