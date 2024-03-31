using CapiGenerator.Parser;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CModel;

public abstract class BaseCAstItem(Guid compilationUnitId)
{
    public readonly Guid CompilationUnitId = compilationUnitId;

    public EnrichingDataStore EnrichingDataStore { get; } = new();

    public virtual void OnSecondPass(CCompilationUnit compilationUnit)
    {
    }
}