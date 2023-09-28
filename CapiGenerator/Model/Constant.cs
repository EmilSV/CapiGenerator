using CppAst;

namespace CapiGenerator.Model
{
    public record class Constant : IGUIDItem
    {
        private readonly Guid _id = Guid.NewGuid();
        public Guid Id => _id;

        public record class ConstantInput
        {
            public required string Name { get; init; }
            public required CppMacro Macro { get; init; }
        }

        public record class ConstantOutput
        {
            public required string Name { get; init; }
            public required string? Value { get; init; }
            public required Type OutputType { get; init; }
        }

        public required ConstantInput Input { get; init; }
        public required ConstantOutput Output { get; init; }
    }
}