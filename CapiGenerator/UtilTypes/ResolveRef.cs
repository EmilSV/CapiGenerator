using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.UtilTypes;

public readonly record struct ResoleRef<TOutput, TKey>
    where TOutput : class
{
    private class ResoleRefClass : BaseResolveRef<TOutput, TKey>
    {
        private TOutput? _output = null;
        private readonly TKey _key = default!;

        public override TKey Key => throw new NotImplementedException();

        public override TOutput? Output => _output;

        public override bool IsOutputResolved() => _output != null;
        public override bool TrySetOutputFromResolver(IResolver<TOutput, TKey> resolver)
        {
            _output ??= resolver.Resolve(_key!);
            return _output != null;
        }

        public ResoleRefClass([DisallowNull] TKey key)
        {
            _key = key;
        }
    }

    private readonly object? _object;

    public TOutput? Output
    {
        get
        {
            if (_object is ResoleRefClass rRef)
            {
                return rRef.Output;
            }

            return _object as TOutput;
        }
    }

    public readonly bool IsOutputResolved() =>
        _object is ResoleRefClass rRef && rRef.IsOutputResolved();

    public readonly bool TrySetOutputFromResolver(IResolver<TOutput, TKey> resolver)
    {
        if (_object is ResoleRefClass rRef)
        {
            return rRef.TrySetOutputFromResolver(resolver);
        }

        return false;
    }

    public ResoleRef([DisallowNull] TKey key)
    {
        _object = new ResoleRefClass(
            key
        );
    }

    public ResoleRef(TOutput? output)
    {
        _object = output;
    }

    public static implicit operator ResoleRef<TOutput, TKey>(TOutput? output) =>
        new(output);
}