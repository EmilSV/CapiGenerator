using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSMethod : BaseCSCallableType,
    ITypeReplace, ICommendableItem, IAttributeAssignableItem, IChildAstItem<BaseCSType>
{
    public required CSTypeInstance ReturnType;
    public string? Name;
    public LazyFormatString? Body;
    public CSAccessModifier AccessModifier;
    public bool IsStatic;
    public bool IsExtern;
    public bool IsOverride;
    public bool IsVirtual;
    public bool IsReadonly;
    public bool IsNew;
    public bool IsAbstract;
    public bool IsAsync;
    public bool IsPartial;
    public CSMethodOperatorModifier OperatorModifier = CSMethodOperatorModifier.None;

    public BaseCSType? ParentType { get; private set; }
    public BaseCSType? Parent => ParentType;

    public CSMethod()
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        CSTypeInstance returnType,
        string name,
        ReadOnlySpan<CSParameter> parameters
    )
    {
        ReturnType = returnType;
        Name = name;
        this.Parameters.AddRange(parameters);
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        IsExtern = (modifiers & CSClassMemberModifier.Extern) != 0;
        IsOverride = (modifiers & CSClassMemberModifier.Override) != 0;
        IsStatic = (modifiers & CSClassMemberModifier.Static) != 0;
        IsVirtual = (modifiers & CSClassMemberModifier.Virtual) != 0;
        IsReadonly = (modifiers & CSClassMemberModifier.ReadOnly) != 0;
        IsNew = (modifiers & CSClassMemberModifier.New) != 0;
        IsAbstract = (modifiers & CSClassMemberModifier.Abstract) != 0;
        IsAsync = (modifiers & CSClassMemberModifier.Async) != 0;
        IsPartial = (modifiers & CSClassMemberModifier.Partial) != 0;
        OperatorModifier = CSMethodOperatorModifierHelper.GetOperatorModifier(modifiers);
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        ICSType returnType,
        string name,
        ReadOnlySpan<CSParameter> parameters
    ) : this(modifiers, new CSTypeInstance(returnType), name, parameters)
    {
    }


    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        CSTypeInstance returnType,
        string name,
        ReadOnlySpan<(CSTypeInstance type, string name)> parameters
    ) : this(modifiers, returnType, name, CSParameter.ParameterArrayFromTurples(parameters))
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        ICSType returnType,
        string name,
        ReadOnlySpan<(CSTypeInstance type, string name)> parameters
    ) : this(modifiers, returnType, name, CSParameter.ParameterArrayFromTurples(parameters))
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        CSTypeInstance returnType,
        string name,
        ReadOnlySpan<(ICSType type, string name)> parameters
    ) : this(modifiers, returnType, name, CSParameter.ParameterArrayFromTurples(parameters))
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        ICSType returnType,
        string name,
        ReadOnlySpan<(ICSType type, string name)> parameters
    ) : this(modifiers, new CSTypeInstance(returnType), name, CSParameter.ParameterArrayFromTurples(parameters))
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        CSTypeInstance returnType,
        ReadOnlySpan<(ICSType type, string name)> parameters
    ) : this(modifiers, returnType, "", CSParameter.ParameterArrayFromTurples(parameters))
    {
    }

    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        CSTypeInstance returnType,
        ReadOnlySpan<(CSTypeInstance type, string name)> parameters
    ) : this(modifiers, returnType, "", CSParameter.ParameterArrayFromTurples(parameters))
    {
    }


    [SetsRequiredMembers]
    public CSMethod(
        CSClassMemberModifier modifiers,
        ICSType returnType,
        ReadOnlySpan<(ICSType type, string name)> parameters
    ) : this(modifiers, returnType, "", CSParameter.ParameterArrayFromTurples(parameters))
    {
    }

    public List<BaseCSAttribute> Attributes { get; } = [];

    public DocComment? Comments { get; set; }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        ReturnType?.OnSecondPass(unit);
        foreach (var parameter in Parameters)
        {
            parameter.OnSecondPass(unit);
        }
    }

    void IChildAstItem<BaseCSType>.SetParent(BaseCSType? parent)
    {
        if (ParentType != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }
        ParentType = parent;
    }

    public void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        var innerReturnType = ReturnType.Type;

        if (innerReturnType is not null)
        {
            if (predicate(innerReturnType, out var newType))
            {
                ReturnType = CSTypeInstance.CopyWithNewType(ReturnType, newType!);
            }
        }
        else
        {
            Console.Error.WriteLine($"method {Name} has null return type and cannot be replaced");
        }

        int count = Parameters.Count;
        for (int i = 0; i < count; i++)
        {
            var parameter = Parameters[i];
            var innerType = parameter.Type.Type;
            if (innerType is null)
            {
                Console.Error.WriteLine($"method {Name} has null parameter type and cannot be replaced");
                continue;
            }
            if (predicate(innerType, out var newType))
            {
                Parameters.TryReplaceAt(i, CSParameter.CopyWithNewType(parameter, newType!));
            }
        }
    }

    public string GetFullName()
    {
        if (ParentType == null)
        {
            throw new InvalidOperationException("Parent type is not set");
        }

        var parentFullName = ParentType.GetFullName();

        string? returnTypeName;

        switch (OperatorModifier)
        {
            case CSMethodOperatorModifier.Explicit:
                if (ReturnType!.Type!.TryGetName(out returnTypeName))
                {
                    return $"{parentFullName}.explicit operator {returnTypeName}";
                }
                else
                {
                    return $"{parentFullName}.explicit operator \"Unknown\"";
                }
            case CSMethodOperatorModifier.Implicit:
                if (ReturnType!.Type!.TryGetName(out returnTypeName))
                {
                    return $"{parentFullName}.implicit operator {returnTypeName}";
                }
                else
                {
                    return $"{parentFullName}.implicit operator \"Unknown\"";
                }
            case CSMethodOperatorModifier.Operator:
                return $"{parentFullName}.operator {Name}";

            default:
                return $"{parentFullName}.{Name}";
        }
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
            var type = parameter.Type?.ToString();
            if (type == null)
            {
                return null;
            }
            parametersTypeNames.Add(type);
        }

        return $"{GetFullName()}({string.Join(",", parametersTypeNames)})";
    }
}
