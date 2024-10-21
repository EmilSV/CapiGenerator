using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public interface ICSFieldLike
{
    string Name { get; }
    InstanceId Id { get; }
    string GetFullName();
}