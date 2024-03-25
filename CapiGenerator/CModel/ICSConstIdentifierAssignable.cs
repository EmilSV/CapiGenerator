namespace CapiGenerator.CModel;

public interface ICConstAssignable
{
    string Name { get; }
    CConstantExpression Expression { get; }
}