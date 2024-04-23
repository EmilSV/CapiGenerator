using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public interface ICSType
{
    public InstanceId Id { get; }
    public bool IsAnonymous { get; }
}