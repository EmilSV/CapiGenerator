using CapiGenerator.Model;

namespace CapiGenerator;

public class CapiParserResult
{
    public required BaseModelRefLookup<Constant> ConstLookup { get; init; }
}