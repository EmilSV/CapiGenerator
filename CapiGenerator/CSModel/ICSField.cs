namespace CapiGenerator.CSModel;

interface ICSField
{
    CSBaseType? Parent { get; }
    string Name { get; }
    string FullName { get; }
}