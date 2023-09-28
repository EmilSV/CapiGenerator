namespace CapiGenerator;


public record class Function
{
    public record class Parameter
    {
        public required string Name { get; init; }
        public required Type OutputType { get; init; }
        public required string InputType { get; init; }
    }

    public required string Name { get; init; }
    public required IReadOnlyList<Parameter> Parameters { get; init; }
}