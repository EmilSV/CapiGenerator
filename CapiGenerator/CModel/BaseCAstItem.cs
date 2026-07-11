using CapiGenerator.Parser;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CModel;

public abstract class BaseCAstItem : ICSecondPassable
{
    private readonly List<object> _secondarySources = [];
    private readonly List<object> _derivatives = [];

    protected BaseCAstItem()
    {
    }

    protected BaseCAstItem(object primarySource)
    {
        PrimarySource = primarySource;
    }

    public object? PrimarySource { get; }
    public IReadOnlyList<object> SecondarySources => _secondarySources;
    public IReadOnlyList<object> Derivatives => _derivatives;
    public EnrichingDataStore EnrichingDataStore { get; } = new();

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

    public virtual void OnSecondPass(CCompilationUnit compilationUnit)
    {
    }
}