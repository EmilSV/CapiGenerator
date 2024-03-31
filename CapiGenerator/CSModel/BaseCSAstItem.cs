using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSAstItem()
{
    public EnrichingDataStore EnrichingDataStore { get; } = new();

    public virtual void OnSecondPass(CSTranslationUnit unit)
    {
        
    }
}