using System.Collections;
using System.Runtime.CompilerServices;

namespace CapiGenerator.UtilTypes;

file static class ChangeCountListBuilder
{
    public static ChangeCountList<T> Create<T>(ReadOnlySpan<T> value)
    {
        return new ChangeCountList<T>(value);
    }
}

[CollectionBuilder(typeof(ChangeCountListBuilder), "Create")]
public sealed class ChangeCountList<T> :
    ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
{
    private readonly List<T> _list = new();
    private volatile uint _changeCounter = 0;

    public ChangeCountList(ReadOnlySpan<T> value)
    {
        _list.AddRange(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NotifyChange()
    {
        _changeCounter++;
    }

    public InstanceId Id { get; } = new();
    public uint ChangeCounter => _changeCounter;

    public int Count => _list.Count;

    public T this[int index]
    {
        get => _list[index];
        set
        {
            _list[index] = value;
            NotifyChange();
        }
    }

    public void Add(T item)
    {
        _list.Add(item);
        NotifyChange();
    }

    public void AddRange(ReadOnlySpan<T> items)
    {
        _list.AddRange(items);
        NotifyChange();
    }

    public void AddRange(IEnumerable<T> items)
    {
        _list.AddRange(items);
        NotifyChange();
    }

    public void Clear()
    {
        var previousCount = _list.Count;
        if (previousCount > 0)
        {
            NotifyChange();
        }
        NotifyChange();
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
        NotifyChange();
    }

    public bool Remove(T item)
    {
        var result = _list.Remove(item);
        if (result)
        {
            NotifyChange();
        }
        return result;
    }

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        _list.Insert(index, item);
        NotifyChange();
    }

    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
        NotifyChange();
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(_list);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    bool ICollection.IsSynchronized => false;
    bool IList.IsFixedSize => false;

    object ICollection.SyncRoot => _list;
    bool ICollection<T>.IsReadOnly => false;
    bool IList.IsReadOnly => false;

    object? IList.this[int index]
    {
        get => _list[index];
        set => _list[index] = (T)value;
    }

    void ICollection.CopyTo(Array array, int index)
    {
        if (array is T[] typedArray)
        {
            CopyTo(typedArray, index);
        }
    }

    bool IList.Contains(object? value)
    {
        if (value is T item)
        {
            return Contains(item);
        }
        return false;
    }

    int IList.IndexOf(object? value)
    {
        if (value is T item)
        {
            return IndexOf(item);
        }
        return -1;
    }

    void IList.Insert(int index, object? value)
    {
        if (value is T item)
        {
            Insert(index, item);
        }
    }

    void IList.Remove(object? value)
    {
        if (value is T item)
        {
            Remove(item);
        }
    }

    int IList.Add(object? value)
    {
        if (value is T item)
        {
            Add(item);
            return Count - 1;
        }
        return -1;
    }

    public struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private List<T>.Enumerator _enumerator;

        public T Current => _enumerator.Current;

        object IEnumerator.Current => _enumerator.Current!;

        internal Enumerator(List<T> list)
        {
            _enumerator = list.GetEnumerator();
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        public bool MoveNext()
        {
            return _enumerator.MoveNext();
        }

        void IEnumerator.Reset()
        {
            ((IEnumerator)_enumerator).Reset();
        }
    }
}