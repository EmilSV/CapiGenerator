namespace CapiGenerator.Model
{
    public record class Constant
    {
        public required string Name { get; init; }
        public required string Value { get; init; }
        public required Type OutputType { get; init; }
        public required string OriginalValue { get; init; }
        public required string InputType { get; init; }
    }
}