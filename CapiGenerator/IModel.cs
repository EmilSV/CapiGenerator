namespace CapiGenerator;

public interface IModel<TSelf>
    where TSelf : class, IModel<TSelf>
{
    public ModelRef<TSelf> ModelRef { get; }
    public string InputName { get; }
    public string CompileUnitNamespace { get; }

    internal void SetOwingFactory(BaseModelRefLookup<TSelf> factory);
}