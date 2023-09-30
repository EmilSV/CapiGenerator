using CppAst;

namespace CapiGenerator;

public record class ParseArgs
{
    public required CppCompilation Compilation { get; init; }
    public required CapiModelLookup Lookups { get; init; }
    public required CsharpOutFolder OutputFolder { get; init; }
    public required string CompileUnitNamespace { get; init; }
}
