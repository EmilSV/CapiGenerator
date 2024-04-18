namespace CapiGenerator.UtilTypes;

public interface INotifyReviver<T>
{
    void OnAdd(T item)
    {
        OnAddRange([item]);
    }
    void OnRemove(T item)
    {
        OnRemoveRange([item]);
    }
    void OnAddRange(ReadOnlySpan<T> items);
    void OnRemoveRange(ReadOnlySpan<T> items);
}