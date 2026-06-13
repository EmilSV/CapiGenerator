using System.Collections;
using CapiGenerator.CSModel;

namespace CapiGenerator.UtilTypes;

public readonly struct ChildList<TChild, TParent>(TParent parent) :
    IEnumerable<TChild>,
    IReadOnlyList<TChild>
    where TParent : class
    where TChild : class, IChildAstItem<TParent>
{
    private static void ThrowIfAlreadyHasParent(IChildAstItem<TParent> child, TParent parent)
    {
        if (child.Parent != null)
        {
            if (!ReferenceEquals(child.Parent, parent))
            {
                throw new InvalidOperationException("Child already has a different parent");
            }
            else
            {
                throw new InvalidOperationException("Child is already in the list");
            }
        }
    }

    private static void ThrowIfNoParent(IChildAstItem<TParent> child)
    {
        if (child.Parent == null)
        {
            throw new InvalidOperationException("Child does not have a parent");
        }
    }

    private readonly List<TChild> _items = [];

    public readonly int Count => _items.Count;

    public TChild this[int index] => _items[index];


    public void Add(TChild child)
    {
        ThrowIfAlreadyHasParent(child, parent);
        child.SetParent(parent);
        _items.Add(child);
    }

    public void AddRange(IEnumerable<TChild> children)
    {
        foreach (var child in children)
        {
            Add(child);
        }
    }

    public void AddRange(ReadOnlySpan<TChild> children)
    {
        foreach (var child in children)
        {
            Add(child);
        }
    }

    public bool TryReplaceAt(int index, TChild child)
    {
        return TryReplaceAt(index, child, out _);
    }

    public bool TryReplaceAt(int index, TChild child, out TChild? oldChild)
    {
        ThrowIfAlreadyHasParent(child, parent);
        if ((uint)index >= (uint)_items.Count)
        {
            oldChild = null;
            return false;
        }
        oldChild = _items[index];
        oldChild.SetParent(null);
        child.SetParent(parent);
        _items[index] = child;
        return true;
    }


    public bool Remove(TChild child)
    {
        ThrowIfNoParent(child);
        if (_items.Remove(child))
        {
            child.SetParent(null);
            return true;
        }
        return false;
    }

    public void RemoveListWhere(Predicate<TChild> match)
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            if (match(_items[i]))
            {
                _items[i].SetParent(null);
                _items.RemoveAt(i);
            }
        }
    }

    public void Clear()
    {
        foreach (var child in _items)
        {
            child.SetParent(null);
        }
        _items.Clear();
    }

    public List<TChild>.Enumerator GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_items).GetEnumerator();
    }

    IEnumerator<TChild> IEnumerable<TChild>.GetEnumerator()
    {
        return ((IEnumerable<TChild>)_items).GetEnumerator();
    }
}
