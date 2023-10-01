using System.Collections.Immutable;
using CapiGenerator.ConstantToken;
using CapiGenerator.ModelFactory;
using CapiGenerator.OutFile;
using CppAst;

namespace CapiGenerator.Model;

public class Constant : IModel<Constant>
{
    public ModelRef<Constant> ModelRef { get; }
    public BaseModelRefLookup<Constant> OwingFactory { get; private set; }
    public string InputName => Input.Name;
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
        public required BaseCSharpOutFile OutputFile { get; set; }
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

    internal void SetOwingFactory(BaseModelRefLookup<Constant> factory)
    {
        OwingFactory = factory;
    }

    void IModel<Constant>.SetOwingFactory(BaseModelRefLookup<Constant> factory)
    {
        OwingFactory = factory;
    }

    internal Constant(
        ModelRef<Constant> modelRef,
        BaseModelRefLookup<Constant> owingFactory,
        ConstantInput input,
        ConstantOutput output)
    {
        ModelRef = modelRef;
        OwingFactory = owingFactory;
        _input = input;
        _output = output;
    }
}