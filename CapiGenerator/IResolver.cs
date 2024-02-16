using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator;

public interface IResolver<TOutput, TKey>
    where TOutput : class, IResolveItem<TKey>
{
    TOutput? Resolve([DisallowNull] TKey key);
}