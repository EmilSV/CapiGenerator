using System.Collections;
using System.Diagnostics.CodeAnalysis;
using CapiGenerator.Parser;

using ObjectType = System.Type;

namespace CapiGenerator.CModel;

public abstract class BaseCAstItem(Guid compilationUnitId)
{
    public readonly Guid CompilationUnitId = compilationUnitId;

    public EnrichingDataStore EnrichingDataStore { get; } = new();

    public virtual void OnSecondPass(CCompilationUnit compilationUnit)
    {
    }
}