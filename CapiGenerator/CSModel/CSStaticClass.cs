using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSStaticClass : BaseCSType, ITypeReplace
{
    private readonly HashSet<CSField> _fields = [];
    private readonly HashSet<CSMethod> _methods = [];

    public bool IsPartial;

    public IReadOnlySet<CSField> Fields => _fields;
    public IReadOnlySet<CSMethod> Methods => _methods;

    public bool AddField(CSField field) => AddItem(_fields, field, field => field.SetParent(this));
    public int AddFields(IEnumerable<CSField> fields) => AddItems(_fields, fields, field => field.SetParent(this));
    public bool RemoveField(CSField field) => RemoveItem(_fields, field, field => field.SetParent(null));
    public int RemoveAllFields(Predicate<CSField>? predicate = null) => RemoveAllItems(_fields, predicate, field => field.SetParent(null));

    public bool AddMethod(CSMethod method) => AddItem(_methods, method, method => method.SetParent(this));
    public int AddMethods(IEnumerable<CSMethod> methods) => AddItems(_methods, methods, method => method.SetParent(this));
    public bool RemoveMethod(CSMethod method) => RemoveItem(_methods, method, method => method.SetParent(null));
    public int RemoveAllMethods(Predicate<CSMethod>? predicate = null) => RemoveAllItems(_methods, predicate, method => method.SetParent(null));

    private static bool AddItem<T>(HashSet<T> set, T item, Action<T>? onAdded = null)
    {
        if (!set.Add(item))
        {
            return false;
        }

        onAdded?.Invoke(item);
        return true;
    }

    private static int AddItems<T>(HashSet<T> set, IEnumerable<T> items, Action<T>? onAdded = null)
    {
        var addedCount = 0;
        foreach (var item in items)
        {
            if (AddItem(set, item, onAdded))
            {
                addedCount++;
            }
        }
        return addedCount;
    }

    private static bool RemoveItem<T>(HashSet<T> set, T item, Action<T>? onRemoved = null)
    {
        if (!set.Remove(item))
        {
            return false;
        }

        onRemoved?.Invoke(item);
        return true;
    }

    private static int RemoveAllItems<T>(HashSet<T> set, Predicate<T>? predicate = null, Action<T>? onRemoved = null)
    {
        if (predicate is not null)
        {
            return set.RemoveWhere(item =>
            {
                if (!predicate(item))
                {
                    return false;
                }

                onRemoved?.Invoke(item);
                return true;
            });
        }

        var removedCount = set.Count;
        if (onRemoved is not null)
        {
            foreach (var item in set)
            {
                onRemoved(item);
            }
        }
        set.Clear();
        return removedCount;
    }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var field in Fields)
        {
            field.OnSecondPass(unit);
        }
        foreach (var method in Methods)
        {
            method.OnSecondPass(unit);
        }
    }

    public void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        foreach (var field in Fields)
        {
            field.ReplaceTypes(predicate);
        }
        foreach (var method in Methods)
        {
            method.ReplaceTypes(predicate);
        }
    }
}
