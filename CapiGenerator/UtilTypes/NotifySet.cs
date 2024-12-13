using System.Collections;
using System.Runtime.InteropServices;

namespace CapiGenerator.UtilTypes;

public sealed class NotifySet<T>(INotifyReviver<T>? notifyReceiver) :
    ICollection<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T>, ICollection
{
    [ThreadStatic] private static List<T>? _tempList;

    private readonly HashSet<T> _set = [];
    private readonly INotifyReviver<T>? _notifyReceiver = notifyReceiver;
    private volatile uint _changeCounter = 0;

    public InstanceId Id { get; } = new();
    public uint ChangeCounter => _changeCounter;

    public int Count => _set.Count;

    public bool Add(T item)
    {
        if (!_set.Add(item))
        {
            return false;
        }
        _changeCounter++;
        _notifyReceiver?.OnAdd(item);
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
            if (_set.Add(item))
            {
                addedItems.Add(item);
            }
        }
        int addedCount = addedItems.Count;

        if (addedCount > 0)
        {
            _changeCounter++;
            _notifyReceiver?.OnAddRange(CollectionsMarshal.AsSpan(addedItems));
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
            if (_set.Add(item))
            {
                addedItems.Add(item);
            }
        }
        int addedCount = addedItems.Count;

        if (addedCount > 0)
        {
            _changeCounter++;
            _notifyReceiver?.OnAddRange(CollectionsMarshal.AsSpan(addedItems));
        }

        _tempList ??= addedItems;
        return addedCount;
    }

    public void Clear()
    {
        var previousCount = _set.Count;
        if (previousCount == 0)
        {
            return;
        }

        var previousList = _set.ToArray();
        _set.Clear();
        _changeCounter++;
        _notifyReceiver?.OnRemoveRange(previousList.AsSpan());
    }

    public bool Contains(T item)
    {
        return _set.Contains(item);
    }

    public void CopyTo(Span<T> span)
    {
        int i = 0;
        foreach (var item in _set)
        {
            if (i >= span.Length)
            {
                break;
            }

            span[i] = item;
        }
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        CopyTo(array.AsSpan(arrayIndex));
    }

    public bool Remove(T item)
    {
        var result = _set.Remove(item);
        if (result)
        {
            _changeCounter++;
            _notifyReceiver?.OnRemove(item);
        }
        return result;
    }


    public int TakeWhere(Predicate<T> predicate, List<T> list)
    {
        List<T>? elementsRemoved = _tempList;
        if (elementsRemoved == null)
        {
            elementsRemoved = [];
        }
        else
        {
            elementsRemoved.Clear();
            _tempList = null;
        }

        int removeCount = _set.RemoveWhere(item =>
        {
            if (predicate(item))
            {
                elementsRemoved.Add(item);
                return true;
            }
            return false;
        });
        _changeCounter++;
        _notifyReceiver?.OnRemoveRange(CollectionsMarshal.AsSpan(elementsRemoved));
        list.AddRange(elementsRemoved);

        _tempList ??= elementsRemoved;
        return removeCount;
    }

    public int TakeWhere(Predicate<T> predicate, NotifySet<T> otherSet)
    {
        List<T>? elementsRemoved = _tempList;
        if (elementsRemoved == null)
        {
            elementsRemoved = [];
        }
        else
        {
            elementsRemoved.Clear();
            _tempList = null;
        }

        int removeCount = _set.RemoveWhere(item =>
        {
            if (predicate(item))
            {
                elementsRemoved.Add(item);
                return true;
            }
            return false;
        });
        _changeCounter++;
        _notifyReceiver?.OnRemoveRange(CollectionsMarshal.AsSpan(elementsRemoved));
        otherSet.AddRange(CollectionsMarshal.AsSpan(elementsRemoved));

        _tempList ??= elementsRemoved;
        return removeCount;
    }

    public int RemoveWhere(Predicate<T> predicate)
    {
        List<T>? elementsRemoved = _tempList;
        if (elementsRemoved == null)
        {
            elementsRemoved = [];
        }
        else
        {
            elementsRemoved.Clear();
            _tempList = null;
        }

        int removeCount = _set.RemoveWhere(item =>
        {
            if (predicate(item))
            {
                elementsRemoved.Add(item);
                return true;
            }
            return false;
        });
        _changeCounter++;
        _notifyReceiver?.OnRemoveRange(CollectionsMarshal.AsSpan(elementsRemoved));
        _tempList ??= elementsRemoved;
        return removeCount;
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(_set);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return _set.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _set.GetEnumerator();
    }

    bool ICollection.IsSynchronized => false;

    object ICollection.SyncRoot => _set;
    bool ICollection<T>.IsReadOnly => false;


    void ICollection.CopyTo(Array array, int index)
    {
        if (array is T[] typedArray)
        {
            CopyTo(typedArray, index);
        }
    }

    void ICollection<T>.Add(T item)
    {
        Add(item);
    }

    public struct Enumerator : IEnumerator<T>, IEnumerator
    {
        private HashSet<T>.Enumerator _enumerator;

        public T Current => _enumerator.Current;

        object IEnumerator.Current => _enumerator.Current!;

        internal Enumerator(HashSet<T> list)
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