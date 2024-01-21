namespace CapiGenerator.CSharpEnrichingData;

public record EnumData
{
    public required string NamespaceName { get; init; }
    public required string EnumName { get; init; }
}