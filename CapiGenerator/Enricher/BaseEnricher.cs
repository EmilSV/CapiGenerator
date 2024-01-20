using CapiGenerator.Parser;

namespace CapiGenerator.Enricher;

public abstract class BaseEnricher
{
    public abstract void Enrich(ReadOnlySpan<CCompilationUnit> compilationUnit);
}