using System.Collections.Immutable;

namespace CapiGenerator.CSModel;

public abstract class BaseCSAttribute
{
    public abstract string GetFullAttributeName();

    public required ImmutableDictionary<string, string> Parameters { get; init; }

    public required ImmutableArray<string> CtorArgs { get; init; }
}