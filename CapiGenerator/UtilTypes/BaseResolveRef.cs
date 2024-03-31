using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.UtilTypes;

public abstract class BaseResolveRef<TOutput, TKey>
    where TOutput : class
{
    public abstract TKey Key { [return: MaybeNull] get; }
    public abstract TOutput? Output { get; }
    public abstract bool IsOutputResolved();
    public abstract bool TrySetOutputFromResolver(IResolver<TOutput, TKey> resolver);
}