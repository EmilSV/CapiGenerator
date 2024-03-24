using System.Collections.Immutable;

namespace CapiGenerator.Writer;

public sealed record CSWriteConfig
{
    public required string OutputDirectory { get; init; }
    public required ImmutableArray<string> Usings { get; init; }
}