using System.Collections.Immutable;

namespace CapiGenerator;


public sealed record class WriterArgs
{
    public required CapiModelLookup Lookups { get; init; }
}