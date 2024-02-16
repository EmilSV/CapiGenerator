namespace CapiGenerator.CModel;

public interface ICType : IResolveItem<string>
{
    string Name { get; }
}