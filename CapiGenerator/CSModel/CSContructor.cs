using CapiGenerator.CSModel.Comments;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSConstructor : BaseCSAstItem, ICommendableItem, IAttributeAssignableItem
{
    public BaseCSType? ParentType { get; private set; }
    public LazyFormatString? Body;

    public DocComment? Comments { get; set; }

    public NotifyList<BaseCSAttribute> Attributes { get; } = new(null);

    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<CSParameter> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters = new(null, parameters);
    }

    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<(CSTypeInstance, string)> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters = new(null, ToParameters(parameters));
    }


    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<(ICSType, string)> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters = new(null, ToParameters(parameters));
    }

    public CSAccessModifier AccessModifier;

    public NotifyList<CSParameter> Parameters { get; }



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

        return $"{GetFullName}({string.Join(",", parametersTypeNames)})";
    }
}