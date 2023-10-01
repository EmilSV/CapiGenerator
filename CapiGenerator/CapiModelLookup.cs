using CapiGenerator.Model;
using CapiGenerator.ModelFactory;

namespace CapiGenerator;

public class CapiModelLookup
{
    public required BaseModelRefLookup<Constant> ConstLookup { get; init; }
    //public readonly GuidRef<Enum>.LookupCollection Lookup = new();
    //public readonly GuidRef<Function>.LookupCollection Lookup = new();
    //public readonly GuidRef<Handel>.LookupCollection Lookup = new();
    //public readonly GuidRef<Struct>.LookupCollection Lookup = new();
}