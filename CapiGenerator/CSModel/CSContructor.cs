using System.Runtime.CompilerServices;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSConstructor :
    BaseCSCallableType, ICommendableItem,
    ITypeReplace, IChildAstItem<BaseCSType>
{
    public BaseCSType? Parent { get; private set; }
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

    void IChildAstItem<BaseCSType>.SetParent(BaseCSType? parent)
    {
        if (Parent != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }
        Parent = parent;
    }

    public string GetFullName()
    {
        if (Parent == null)
        {
            throw new InvalidOperationException("Parent type is not set");
        }

        return $"{Parent.GetFullName()}.{Parent.Name}";
    }

    public string? GetFullNameWithParameters()
    {
        if (Parent == null)
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

    public void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        int count = Parameters.Count;
        for (int i = 0; i < count; i++)
        {
            var parameter = Parameters[i];
            var innerType = parameter.Type.Type;
            if (innerType is null)
            {
                var fullName = Parent is not null ? GetFullName() : "Unknown";
                Console.Error.WriteLine($"CSConstructor {fullName} has null parameter type and cannot be replaced");
                continue;
            }
            if (predicate(innerType, out var newType))
            {
                Parameters.TryReplaceAt(i, CSParameter.CopyWithNewType(parameter, newType!));
            }
        }
    }

}
