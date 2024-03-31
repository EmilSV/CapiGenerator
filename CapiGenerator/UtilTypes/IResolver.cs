using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.UtilTypes;

public interface IResolver<TOutput, TKey>
    where TOutput : class
{
    TOutput? Resolve([DisallowNull] TKey key);
}