using System.Collections.Immutable;
namespace CapiGenerator.CSModel;

public sealed class CSAttribute<T> : BaseCSAttribute
    where T : System.Attribute
{
    public override string GetFullAttributeName()
    {
        return typeof(T).FullName!;
    }

    public static CSAttribute<T> Create(IEnumerable<string> attributeCtorArgs, IEnumerable<KeyValuePair<string, string>> attributeParameters)
    {
        return new CSAttribute<T>
        {
            CtorArgs = attributeCtorArgs.ToImmutableArray(),
            Parameters = attributeParameters.ToImmutableDictionary()
        };
    }
}