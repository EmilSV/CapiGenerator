using CapiGenerator.Parser;

namespace CapiGenerator.CModel;


public sealed class CEnumField(
    Guid compilationUnitId, string name, CConstantExpression expression) :
    BaseCAstItem(compilationUnitId), ICType
{
    public string Name => name;
    public CConstantExpression Expression => expression;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        expression.OnSecondPass(compilationUnit);
    }

    string IResolveItem<string>.GetResolveKey() => Name;
}