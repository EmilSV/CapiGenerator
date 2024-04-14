using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSAstItem()
{
    public EnrichingDataStore EnrichingDataStore { get; } = new();

    private volatile uint _changeCount = 0;
    public InstanceId Id { get; } = new();
    public uint ChangeCount => _changeCount;

    protected void NotifyChange()
    {
        _changeCount++;
    }


    public virtual void OnSecondPass(CSTranslationUnit unit)
    {

    }
}