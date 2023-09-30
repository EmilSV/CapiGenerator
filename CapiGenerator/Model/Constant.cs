using System.Collections.Immutable;
using CapiGenerator.ConstantToken;
using CppAst;

namespace CapiGenerator.Model
{
    public class Constant : IGUIDItem<Constant>
    {
        public GuidRef<Constant> Id { get; }
        public string OriginalName => Input.Name;
        public string CompileUnitNamespace => Input.CompileUnitNamespace;

        public class ConstantInput : IEquatable<ConstantInput>
        {
            public required string Name { get; init; }
            public required string CompileUnitNamespace { get; init; }
            public required CppMacro Macro { get; init; }

            public bool Equals(ConstantInput? other) =>
                other is not null &&
                Name == other.Name &&
                CompileUnitNamespace == other.CompileUnitNamespace;

            public override bool Equals(object? obj) => Equals(obj as ConstantInput);

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, CompileUnitNamespace);
            }
        }

        public class ConstantOutput
        {
            public required string Name { get; set; }
            public required string OutputClassName { get; set; }
            public required CSharpOutFile OutputFile { get; set; }
            public required ImmutableArray<BaseConstantToken> Tokens { get; set; }
            public ConstWriter? Writer { get; set; }
        }

        private readonly ConstantInput _input;
        private readonly ConstantOutput _output;

        public ConstantInput Input => _input;
        public ConstantOutput Output => _output;

        public ConstantType ResolveOutputType(IConstantTypeResolver resolver)
        {
            var type = resolver.ResolveType(this);
            return type;
        }

        public Constant(GuidRef<Constant>.LookupCollection lookup, ConstantInput input, ConstantOutput output)
        {
            Id = lookup.Get(input.Name, input.CompileUnitNamespace);
            _input = input;
            _output = output;
            lookup.Add(this);
        }
    }
}