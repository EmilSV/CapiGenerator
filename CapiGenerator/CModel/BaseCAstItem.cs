using CapiGenerator.Parser;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CModel;

public abstract class BaseCAstItem : ICSecondPassable
{
    private readonly List<object> _secondarySources = [];

    protected BaseCAstItem()
    {
    }

    protected BaseCAstItem(object primarySource)
    {
        PrimarySource = primarySource;
    }

    public object? PrimarySource { get; }
    public IReadOnlyList<object> SecondarySources => _secondarySources;
    public EnrichingDataStore EnrichingDataStore { get; } = new();

    public void AddSecondarySource(object source)
    {
        _secondarySources.Add(source);
    }

    public void AddSecondarySources(IEnumerable<object> sources)
    {
        _secondarySources.AddRange(sources);
    }

    public virtual void OnSecondPass(CCompilationUnit compilationUnit)
    {
    }
}