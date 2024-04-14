using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public interface ICSFieldLike
{
    string Name { get; }
    InstanceId Id { get; }
    uint ChangeCount { get; }
}