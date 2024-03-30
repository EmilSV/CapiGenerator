using System.Collections;

namespace CapiGenerator.Collection;


public sealed class HistoricList<T> : IReadOnlyList<T>
{
    public delegate void ReplacePredicate(ref T value, int index, out bool remove);

    private readonly List<T[]> _values = [];
    private int _version;

    public HistoricList(ReadOnlySpan<T> value)
    {
        _values.Add([.. value]);
    }

    public void Add(ReadOnlySpan<T> value)
    {
        _values.Add([.. CurrentValue, .. value]);
        _version++;
    }

    public int RemoveWhere(Func<T, bool> predicate)
    {
        T[] originalValues = _values[^1];
        int version = _version;
        List<T> newValues = new(originalValues.Length);

        for (int i = 0; i < originalValues.Length; i++)
        {
            if (!predicate(originalValues[i]))
            {
                if (version != _version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                newValues.Add(originalValues[i]);
            }
        }

        _values.Add([.. newValues]);

        return originalValues.Length - newValues.Count;
    }

    public int RemoveWhere(Func<T, int, bool> predicate)
    {
        T[] originalValues = _values[^1];
        int version = _version;
        List<T> newValues = new(originalValues.Length);

        for (int i = 0; i < originalValues.Length; i++)
        {
            if (!predicate(originalValues[i], i))
            {
                if (version != _version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                newValues.Add(originalValues[i]);
            }
        }

        _values.Add([.. newValues]);

        return originalValues.Length - newValues.Count;
    }

    public int ReplaceWhere(ReplacePredicate predicate)
    {
        T[] originalValues = _values[^1];
        int version = _version;
        List<T> newValues = new(originalValues.Length);

        for (int i = 0; i < originalValues.Length; i++)
        {
            T value = originalValues[i];
            predicate(ref value, i, out bool remove);
            if (!remove)
            {
                if (version != _version)
                {
                    throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
                }
                newValues.Add(originalValues[i]);
            }
        }

        _values.Add([.. newValues]);

        return originalValues.Length - newValues.Count;
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(this);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private ReadOnlySpan<T> CurrentValue
    {
        get
        {
            if (_values.Count == 0)
            {
                throw new InvalidOperationException("No values in the collection");
            }

            return _values[^1];
        }
    }

    public int Count => CurrentValue.Length;

    public T this[int index] => CurrentValue[index];

    public IReadOnlyList<IReadOnlyList<T>> GetHistoricValues()
    {
        return _values;
    }

    public static implicit operator ReadOnlySpan<T>(HistoricList<T> historicValues)
    {
        return historicValues.CurrentValue;
    }


    public struct Enumerator : IEnumerator<T>, IEnumerator
    {
        internal static IEnumerator<T>? s_emptyEnumerator;

        private readonly HistoricList<T> _list;
        private readonly T[] _array;
        private int _index;
        private readonly int _version;
        private T? _current;

        internal Enumerator(HistoricList<T> list)
        {
            _list = list;
            _array = list._values[^1];
            _index = 0;
            _version = list._version;
            _current = default;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            var localList = _list;
            var array = _array;

            if (_version == localList._version && ((uint)_index < (uint)array.Length))
            {
                _current = array[_index];
                _index++;
                return true;
            }
            return MoveNextRare();
        }

        private bool MoveNextRare()
        {
            if (_version != _list._version)
            {
                throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
            }

            _index = _array.Length + 1;
            _current = default;
            return false;
        }

        public T Current => _current!;

        object? IEnumerator.Current
        {
            get
            {
                if (_index == 0 || _index == _array.Length + 1)
                {
                    throw new InvalidOperationException("Enumeration has either not started or has already finished.");
                }
                return Current;
            }
        }

        void IEnumerator.Reset()
        {
            if (_version != _list._version)
            {
                throw new InvalidOperationException("Collection was modified; enumeration operation may not execute.");
            }

            _index = 0;
            _current = default;
        }
    }
}