using CapiGenerator.CModel;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSEnum : BaseCSType, ITypeReplace
{
    private readonly List<CSEnumField> _values = [];

    public CSPrimitiveType Type = CSPrimitiveType.Instances.Int;

    public IReadOnlyList<CSEnumField> Values => _values;

    public bool AddValue(CSEnumField value)
    {
        if (_values.Contains(value))
        {
            return false;
        }

        value.SetParent(this);
        _values.Add(value);
        return true;
    }

    public int AddValues(IEnumerable<CSEnumField> values)
    {
        var addedCount = 0;
        foreach (var value in values)
        {
            if (AddValue(value))
            {
                addedCount++;
            }
        }
        return addedCount;
    }

    public void AddValues(ReadOnlySpan<CSEnumField> values)
    {
        foreach (var value in values)
        {
            AddValue(value);
        }
    }

    public bool RemoveValue(CSEnumField value)
    {
        if (!_values.Remove(value))
        {
            return false;
        }

        value.SetParent(null);
        return true;
    }

    public int RemoveAllValues(Predicate<CSEnumField>? predicate = null)
    {
        if (predicate is null)
        {
            var removedCount = _values.Count;
            foreach (var value in _values)
            {
                value.SetParent(null);
            }
            _values.Clear();
            return removedCount;
        }

        var removed = 0;
        for (int i = _values.Count - 1; i >= 0; i--)
        {
            var value = _values[i];
            if (!predicate(value))
            {
                continue;
            }

            _values.RemoveAt(i);
            value.SetParent(null);
            removed++;
        }
        return removed;
    }

    public bool TryReplaceValueAt(int index, CSEnumField value)
    {
        if ((uint)index >= (uint)_values.Count)
        {
            return false;
        }

        var oldValue = _values[index];
        if (ReferenceEquals(oldValue, value))
        {
            return true;
        }

        if (_values.Contains(value))
        {
            return false;
        }

        value.SetParent(this);
        oldValue.SetParent(null);
        _values[index] = value;
        return true;
    }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var value in Values)
        {
            value.OnSecondPass(unit);
        }
    }

    public void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        if (predicate(Type, out var newType))
        {
            if (newType is CSPrimitiveType primitiveType)
            {
                Type = primitiveType;
            }
            else
            {
                Console.Error.WriteLine($"Type {newType} is not supported for enum");
            }
        }
    }
}
