using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.UtilTypes;

public class ResoleRef<TOutput, TKey> : BaseResolveRef<TOutput, TKey>
    where TOutput : class
{
    private TOutput? _output = null;
    private readonly TKey _key = default!;

    public override TKey Key
    {
        [return: MaybeNull]
        get => _key;
    }
    public override TOutput? Output => _output;

    public override bool IsOutputResolved() => _output != null;

    public override bool TrySetOutputFromResolver(IResolver<TOutput, TKey> resolver)
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