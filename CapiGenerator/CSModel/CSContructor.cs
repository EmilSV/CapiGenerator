using CapiGenerator.CSModel.Comments;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSConstructor : BaseCSCallableType, ICommendableItem
{
    public BaseCSType? ParentType { get; private set; }
    public LazyFormatString? Body;

    public DocComment? Comments { get; set; }

    public List<BaseCSAttribute> Attributes { get; } = [];

    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<CSParameter> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters.AddRange(parameters);
    }

    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<(CSTypeInstance, string)> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters.AddRange(CSParameter.ParameterArrayFromTurples(parameters));
    }


    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<(ICSType, string)> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters.AddRange(CSParameter.ParameterArrayFromTurples(parameters));
    }

    public CSAccessModifier AccessModifier;

    internal void SetParent(BaseCSType? parent)
    {
        if (ParentType != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }
        ParentType = parent;
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

        List<string> parametersTypeNames = [];
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
