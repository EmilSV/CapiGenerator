using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Parser;
using CppAst;

namespace CapiGenerator.CModel;


public sealed class CEnumField(object primarySource, string name, CConstantExpression expression) :
    BaseCAstItem(primarySource), ICConstAssignable
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

    public static CEnumField From(CppEnumItem item) =>
        new(item, item.Name, [new CConstLiteralToken(item.Value.ToString())]);
}
