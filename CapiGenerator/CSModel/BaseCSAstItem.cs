using CapiGenerator.CModel;
using CapiGenerator.Parser;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSAstItem : ICSSecondPassable
{
    private readonly List<object> _secondarySources = [];
    private readonly List<object> _derivatives = [];

    protected BaseCSAstItem()
    {
    }

    protected BaseCSAstItem(object? primarySource)
    {
        PrimarySource = primarySource;
    }

    public object? PrimarySource { get; }
    public IReadOnlyList<object> SecondarySources => _secondarySources;
    public IReadOnlyList<object> Derivatives => _derivatives;
    public EnrichingDataStore EnrichingDataStore { get; } = new();
    public InstanceId Id { get; } = new();

    public void AddSecondarySource(object source)
    {
        _secondarySources.Add(source);
    }

    public void AddSecondarySources(IEnumerable<object> sources)
    {
        _secondarySources.AddRange(sources);
    }

    public void AddDerivative(object derivative)
    {
        _derivatives.Add(derivative);
    }

    public void AddDerivatives(IEnumerable<object> derivatives)
    {
        _derivatives.AddRange(derivatives);
    }

    public virtual void OnSecondPass(CSTranslationUnit unit)
    {

    }
}
