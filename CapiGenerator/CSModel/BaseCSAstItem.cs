using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public abstract class BaseCSAstItem()
{
    public EnrichingDataStore EnrichingDataStore { get; } = new();

    public virtual void OnSecondPass(CSTranslationUnit unit)
    {
        
    }
}