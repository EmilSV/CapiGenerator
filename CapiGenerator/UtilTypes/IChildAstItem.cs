namespace CapiGenerator.UtilTypes;


public interface IChildAstItem<TParent>
    where TParent : class
{
    internal void SetParent(TParent? parent);
    TParent? Parent { get; }
}

internal static class IChildAstItem
{
    private static void ThrowIfAlreadyHasParent<TParent>(IChildAstItem<TParent> child, TParent parent)
        where TParent : class
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

    private static void ThrowIfNoParent<TParent>(IChildAstItem<TParent> child)
        where TParent : class
    {
        if (child.Parent == null)
        {
            throw new InvalidOperationException("Child does not have a parent");
        }
    }

    public static void AddToParentList<TParent>(this IChildAstItem<TParent> child, List<IChildAstItem<TParent>> list, TParent parent)
        where TParent : class
    {
        ThrowIfAlreadyHasParent(child, parent);
        child.SetParent(parent);
        list.Add(child);
    }

    public static void RemoveFromParentList<TParent>(this IChildAstItem<TParent> child, List<IChildAstItem<TParent>> list)
        where TParent : class
    {
        ThrowIfNoParent(child);
        if (list.Remove(child))
        {
            child.SetParent(null);
        }
    }

    public static int RemoveParentListWhere<TParent>(this List<IChildAstItem<TParent>> list, Func<IChildAstItem<TParent>, bool> predicate)
        where TParent : class
    {
        int count = 0;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            var child = list[i];
            if (predicate(child))
            {
                child.SetParent(null);
                list.RemoveAt(i);
                count++;
            }
        }
        return count;
    }

    public static void ClearParentList<TParent>(this List<IChildAstItem<TParent>> list)
        where TParent : class
    {
        foreach (var child in list)
        {
            child.SetParent(null);
        }
        list.Clear();
    }

}
