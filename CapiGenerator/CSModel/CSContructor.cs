using CapiGenerator.CSModel.Comments;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSConstructor : BaseCSAstItem, ICommendableItem
{
    private readonly List<CSParameter> _parameters = [];

    public BaseCSType? ParentType { get; private set; }
    public LazyFormatString? Body;

    public DocComment? Comments { get; set; }

    public List<BaseCSAttribute> Attributes { get; } = [];

    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<CSParameter> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        AddParameters(parameters);
    }

    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<(CSTypeInstance, string)> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        AddParameters(ToParameters(parameters).AsSpan());
    }


    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<(ICSType, string)> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        AddParameters(ToParameters(parameters).AsSpan());
    }

    public CSAccessModifier AccessModifier;

    public IReadOnlyList<CSParameter> Parameters => _parameters;

    public void AddParameter(CSParameter parameter)
    {
        parameter.SetParentMethod(this);
        _parameters.Add(parameter);
    }

    public void AddParameters(IEnumerable<CSParameter> parameters)
    {
        _parameters.AddRange(parameters);
    }

    public void AddParameters(ReadOnlySpan<CSParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            AddParameter(parameter);
        }
    }

    public bool RemoveParameter(CSParameter parameter)
    {
        return _parameters.Remove(parameter);
    }

    public int RemoveAllParameters(Predicate<CSParameter>? predicate = null)
    {
        if (predicate is null)
        {
            var removedCount = _parameters.Count;
            _parameters.Clear();
            return removedCount;
        }

        return _parameters.RemoveAll(predicate);
    }

    public bool TryReplaceParameterAt(int index, CSParameter parameter)
    {
        if ((uint)index >= (uint)_parameters.Count)
        {
            return false;
        }

        _parameters[index] = parameter;
        return true;
    }

    internal void SetParent(BaseCSType? parent)
    {
        if (ParentType != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }
        ParentType = parent;
    }

    private static CSParameter[] ToParameters(ReadOnlySpan<(CSTypeInstance type, string name)> parameters)
    {
        if (parameters.Length == 0)
        {
            return [];
        }

        var array = new CSParameter[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var (type, name) = parameters[i];
            array[i] = new(type, name);
        }
        return array;
    }

    private static CSParameter[] ToParameters(ReadOnlySpan<(ICSType type, string name)> parameters)
    {
        if (parameters.Length == 0)
        {
            return [];
        }

        var array = new CSParameter[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var (type, name) = parameters[i];
            array[i] = new(new(type), name);
        }
        return array;
    }

    public string GetFullName()
    {
        if (ParentType == null)
        {
            throw new InvalidOperationException("Parent type is not set");
        }

        return $"{ParentType.GetFullName()}.{ParentType.Name}";
    }

    public string? GetFullNameWithParameters()
    {
        if (ParentType == null)
        {
            throw new InvalidOperationException("Parent type is not set");
        }

        List<string> parametersTypeNames = new();
        foreach (var parameter in Parameters)
        {
            if (parameter.Type?.Type?.TryGetName(out var typeName) == true)
            {
                parametersTypeNames.Add(typeName);
            }
            else
            {
                return null;
            }
        }

        return $"{GetFullName()}({string.Join(",", parametersTypeNames)})";
    }
}
