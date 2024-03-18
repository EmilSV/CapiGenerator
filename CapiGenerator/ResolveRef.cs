using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator;

public sealed class ResoleRef<TOutput, TKey>
    where TOutput : class
{
    private TOutput? _output = null;
    private readonly TKey _key = default!;

    public TKey Key
    {
        [return: MaybeNull]
        get => _key;
    }
    public TOutput? Output => _output;

    public bool IsOutputResolved() => _output != null;

    public bool TrySetOutputFromResolver(IResolver<TOutput, TKey> resolver)
    {
        _output ??= resolver.Resolve(_key!);
        return _output != null;
    }

    public ResoleRef([DisallowNull] TKey key)
    {
        _key = key;
    }

    public ResoleRef(TOutput output)
    {
        _output = output;
    }
}