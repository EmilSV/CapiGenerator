using CapiGenerator.ConstantToken;
using CapiGenerator.OutFile;
using CppAst;
using System.Collections.Immutable;
namespace CapiGenerator.Model;


public record class Enum : IModel<Enum>
{
    public record class Item
    {
        public required string Name { get; init; }
        public required ImmutableArray<BaseConstantToken> ValueTokens { get; init; }
    }

    public sealed class EnumInput
    {
        public required string Name { get; init; }
        public required string CompileUnitNamespace { get; init; }
        public required CppEnum Enum { get; init; }

        public bool Equals(EnumInput? other) =>
            other is not null &&
            Name == other.Name &&
            CompileUnitNamespace == other.CompileUnitNamespace;

        public override bool Equals(object? obj) => Equals(obj as EnumInput);

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, CompileUnitNamespace);
        }
    }

    public sealed class EnumOutput
    {
        public required string Name { get; init; }
        public required BaseCSharpOutFile OutputFile { get; set; }
        public required ImmutableArray<Item> Members { get; init; }
    }

    private readonly EnumInput _input;
    private readonly EnumOutput _output;

    public EnumInput Input => _input;
    public EnumOutput Output => _output;

    public ModelRef<Enum> ModelRef { get; }
    public BaseModelRefLookup<Enum> OwingFactory { get; private set; }
    public string InputName => Input.Name;
    public string CompileUnitNamespace => Input.CompileUnitNamespace;


    internal Enum(
        ModelRef<Enum> modelRef,
        BaseModelRefLookup<Enum> owingFactory,
        EnumInput input,
        EnumOutput output)
    {
        ModelRef = modelRef;
        OwingFactory = owingFactory;
        _input = input;
        _output = output;
    }
    internal void SetOwingFactory(BaseModelRefLookup<Enum> factory)
    {
        OwingFactory = factory;
    }

    void IModel<Enum>.SetOwingFactory(BaseModelRefLookup<Enum> factory)
    {
        OwingFactory = factory;
    }
}