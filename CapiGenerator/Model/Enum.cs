namespace CapiGenerator.Model;


public record class Enum
{
    public record class Member
    {
        public required string Name { get; init; }
        public required string Value { get; init; }
    }

    public required string Name { get; init; }
    public required IReadOnlyList<Member> Members { get; init; }
    public required Type OutputType { get; init; }
    public required string InputType { get; init; }
}