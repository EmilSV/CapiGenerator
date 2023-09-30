namespace CapiGenerator;

public interface IGUIDItem<TSelf>
    where TSelf : class, IGUIDItem<TSelf>
{
    public GuidRef<TSelf> Id { get; }
    public string OriginalName { get; }
    public string CompileUnitNamespace { get; }
}