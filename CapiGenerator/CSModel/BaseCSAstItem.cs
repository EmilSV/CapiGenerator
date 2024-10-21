using CapiGenerator.CModel;
using CapiGenerator.Parser;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSAstItem() : ICSSecondPassable
{
    public EnrichingDataStore EnrichingDataStore { get; } = new();
    public InstanceId Id { get; } = new();

    public virtual void OnSecondPass(CSTranslationUnit unit)
    {

    }
}