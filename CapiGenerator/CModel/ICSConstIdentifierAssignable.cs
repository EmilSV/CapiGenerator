namespace CapiGenerator.CModel;

public interface ICConstAssignable
{
    string Name { get; }
    CConstantType GetCConstantType();
}