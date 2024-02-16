using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CSModel;

namespace CapiGenerator;

public sealed class ResoleRef<TOutput, TKey>
    where TOutput : class, IResolveItem<TKey>
{
    private TOutput? _output = null;
    private readonly TKey _key = default!;
    private readonly bool _isKeySet;

    public TKey Key
    {
        [return: MaybeNull]
        get => _key;
    }
    public TOutput? Output => _output;

    public bool IsOutputResolved() => _output != null;

    public bool TrySetOutputFromResolver(IResolver<TOutput, TKey> resolver)
    {
        if (_isKeySet)
        {
            _output = resolver.Resolve(_key!);
        }
        return _output != null;
    }

    public ResoleRef([DisallowNull] TKey key)
    {
        _key = key!;
        _isKeySet = true;
    }

    public ResoleRef(TOutput output)
    {
        _output = output;
        _isKeySet = false;
    }
}