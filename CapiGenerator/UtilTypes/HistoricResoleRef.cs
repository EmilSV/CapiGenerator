using System.Diagnostics.CodeAnalysis;

namespace CapiGenerator.UtilTypes;

public sealed class HistoricResoleRef<TOutput, TKey> : BaseResolveRef<TOutput, TKey>, IHistoricChangeNotify<TOutput>
    where TOutput : class
{
    private readonly List<TOutput> _values = [];
    private bool _keySet = false;
    private bool _outputSet = false;
    private readonly TKey _key = default!;
    private event Action? TypelessOnChange;

    public event Action<TOutput>? OnChange;

    public HistoricResoleRef([DisallowNull] TKey key)
    {
        _key = key;
        _keySet = true;
    }

    public HistoricResoleRef(TOutput output)
    {
        _values.Add(output);
        _outputSet = true;
    }

    event Action? IHistoricChangeNotify.OnChange
    {
        add
        {
            TypelessOnChange += value;
        }

        remove
        {
            TypelessOnChange -= value;
        }
    }

    public override TKey Key
    {
        [return: MaybeNull]
        get
        {
            return _key;
        }
    }

    public override TOutput? Output => _values.Count > 0 ? _values[^1] : null;

    public void SetOutputValue(TOutput output)
    {
        _values.Add(output);
        OnChange?.Invoke(output);
        TypelessOnChange?.Invoke();
    }

    public override bool IsOutputResolved()
    {
        return _values.Count > 0;
    }


    public IReadOnlyList<TOutput> GetHistoricValues()
    {
        return _values;
    }

    public override bool TrySetOutputFromResolver(IResolver<TOutput, TKey> resolver)
    {
        if (_keySet && !_outputSet)
        {
            var output = resolver.Resolve(_key!);
            if (output == null)
            {
                return false;
            }
            _outputSet = true;
            _values.Insert(0, output);
            if (_values.Count == 1)
            {
                OnChange?.Invoke(output);
                TypelessOnChange?.Invoke();
            }

            return true;
        }

        return _outputSet;
    }

}
