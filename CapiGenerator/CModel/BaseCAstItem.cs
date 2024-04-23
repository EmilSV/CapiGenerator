using CapiGenerator.Parser;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CModel;

public abstract class BaseCAstItem : ICSecondPassable 
{
    public EnrichingDataStore EnrichingDataStore { get; } = new();

    public virtual void OnSecondPass(CCompilationUnit compilationUnit)
    {
    }
}