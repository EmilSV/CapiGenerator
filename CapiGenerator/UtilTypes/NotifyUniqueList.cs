using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CapiGenerator.UtilTypes;

public sealed class NotifyUniqueList<T>(INotifyReviver<T> notifyReceiver) :
    ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IList
{
    [ThreadStatic] private static List<T>? _tempList;

    private readonly List<T> _list = [];
    private readonly INotifyReviver<T> _notifyReceiver = notifyReceiver;
    private volatile uint _changeCounter = 0;

    public InstanceId Id { get; } = new();
    public uint ChangeCounter => _changeCounter;

    public int Count => _list.Count;

    public T this[int index]
    {
        get => _list[index];
    }

    public bool Add(T item)
    {
        if (_list.Contains(item))
        {
            return false;
        }

        _list.Add(item);
        _changeCounter++;
        _notifyReceiver.OnAdd(item);
        return true;
    }

    public int AddRange(ReadOnlySpan<T> items)
    {
        List<T>? addedItems = _tempList;
        if (addedItems == null)
        {
            addedItems = [];
        }
        else
        {
            addedItems.Clear();
            _tempList = null;
        }

        foreach (var item in items)
        {
            if (_list.Contains(item))
            {
                continue;
            }

            _list.Add(item);
            addedItems.Add(item);
        }

        int addedCount = addedItems.Count;

        if (addedCount > 0)
        {
            _changeCounter++;
            _notifyReceiver.OnAddRange(CollectionsMarshal.AsSpan(addedItems));
        }

        _tempList ??= addedItems;

        return addedCount;
    }

    public int AddRange(IEnumerable<T> items)
    {
        List<T>? addedItems = _tempList;
        if (addedItems == null)
        {
            addedItems = [];
        }
        else
        {
            addedItems.Clear();
            _tempList = null;
        }

        foreach (var item in items)
        {
            if (_list.Contains(item))
            {
                continue;
            }

            _list.Add(item);
            addedItems.Add(item);
        }

        int addedCount = addedItems.Count;

        if (addedItems.Count > 0)
        {
            _changeCounter++;
            _notifyReceiver.OnAddRange(CollectionsMarshal.AsSpan(addedItems));
        }

        _tempList ??= addedItems;

        return addedCount;
    }

    public void Clear()
    {
        var previousCount = _list.Count;
        if (previousCount > 0)
        {
            var previousItems = _list.ToArray();
            _changeCounter++;
            _notifyReceiver.OnRemoveRange(previousItems);
        }
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        var result = _list.Remove(item);
        if (result)
        {
            _changeCounter++;
            _notifyReceiver.OnRemove(item);
        }
        return result;
    }

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        if (_list.Contains(item))
        {
            return;
        }

        _list.Insert(index, item);
        _changeCounter++;
        _notifyReceiver.OnAdd(item);
    }

    public bool TryReplaceAt(int index, T item)
    {
        if (index < 0 || index >= _list.Count)
        {
            return false;
        }
        var previousItem = _list[index];
        _list[index] = item;
        _changeCounter++;
        _notifyReceiver.OnRemove(previousItem);
        _notifyReceiver.OnAdd(item);
        return true;
    }

    public void RemoveAt(int index)
    {
        var item = _list[index];
        _list.RemoveAt(index);
        _changeCounter++;
        _notifyReceiver.OnRemove(item);
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
        set => _list[index] = (T)value!;
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

    void ICollection<T>.Add(T item)
    {
        Add(item);
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