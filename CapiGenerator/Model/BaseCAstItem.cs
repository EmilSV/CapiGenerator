using CapiGenerator.Parser;

namespace CapiGenerator.Model;

public abstract class BaseCAstItem(Guid compilationUnitId)
{
    public readonly Guid CompilationUnitId = compilationUnitId;
    public virtual void OnSecondPass(CCompilationUnit compilationUnit)
    {
    }
}