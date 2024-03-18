namespace CapiGenerator.CModel;

public interface ICType
{
    string? Name { get; }
    bool IsAnonymous { get => false; }
}