namespace CapiGenerator.CSModel;

public interface ICSField
{
    CSBaseType? Parent { get; }
    string Name { get; }
    string FullName { get; }
}