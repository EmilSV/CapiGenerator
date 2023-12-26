using CapiGenerator.Model;

namespace CapiGenerator;


public sealed record GeneratorOutputArgs
{
    public required  IReadOnlyList<Constant> OtherConstants;
}