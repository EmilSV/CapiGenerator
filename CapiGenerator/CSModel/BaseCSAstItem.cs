using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSAstItem()
{
    public EnrichingDataStore EnrichingDataStore { get; } = new();

    private volatile uint _changeCounter = 0;
    public readonly InstanceId Id = new();
    public uint ChangeCount => _changeCounter;
    
    protected void NotifyChange()
    {
        _changeCounter++;
    }


    public virtual void OnSecondPass(CSTranslationUnit unit)
    {

    }
}