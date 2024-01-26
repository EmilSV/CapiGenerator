namespace CapiGenerator.CSModel;

public abstract class BaseCSAstItem()
{
    public EnrichingDataStore EnrichingDataStore { get; } = new();
}