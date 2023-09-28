namespace CapiGenerator;

public record Handel
{
    public required string Name { get; init; }
    public required string InputType { get; init; }
    public Function? FreeFunction { get; init; }
}