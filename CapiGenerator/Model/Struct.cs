namespace CapiGenerator.Model;

public record class Struct
{
    public record class Member
    {
        public required string Name { get; init; }
        public required StructMemberType OutputType { get; init; }
        public required string InputType { get; init; }
    }

    public required string Name { get; init; }
    public required IReadOnlyList<Member> Members { get; init; }
    public required string InputType { get; init; }
}