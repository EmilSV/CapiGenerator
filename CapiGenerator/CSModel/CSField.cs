using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSField : BaseCSAstItem,
    ICSFieldLike, ITypeReplace
{
    public required string Name { get; set; }
    public required CSTypeInstance Type;
    public CSDefaultValue DefaultValue;
    public bool IsConst;
    public bool IsStatic;
    public bool IsReadOnly;
    public bool IsRequired;
    public CommentSummery? Comments { get; set; }

    public CSAccessModifier AccessModifier;

    public CSPropertyBody? GetterBody;
    public CSPropertyBody? SetterBody;

    public BaseCSType? ParentType { get; private set; }


    public CSField()
    {

    }

#pragma warning disable 8618
    [SetsRequiredMembers]
    public CSField(CSClassMemberModifier modifier, CSTypeInstance type, string name)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifier);
        Type = type;
        Name = name;
        IsConst = (modifier & CSClassMemberModifier.Const) != 0;
        IsReadOnly = (modifier & CSClassMemberModifier.ReadOnly) != 0;
        IsStatic = (modifier & CSClassMemberModifier.Static) != 0;
    }

    [SetsRequiredMembers]
    public CSField(CSTypeInstance type, string name)
    {
        AccessModifier = CSAccessModifier.Private;
        Type = type;
        Name = name;
    }
#pragma warning restore 8618

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        Type.OnSecondPass(unit);
        DefaultValue.OnSecondPass(unit);
    }

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

        return $"{ParentType.GetFullName()}.{Name}";
    }

    public void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        var innerType = Type.Type;
        if (innerType is null)
        {
            Console.Error.WriteLine($"field {Name} has null type and cannot be replaced");
            return;
        }

        if (innerType is BaseCSAnonymousType anonymousType)
        {
            anonymousType.ReplaceTypes(predicate);
            return;
        }

        if (predicate(innerType, out var newType))
        {
            Type = CSTypeInstance.CopyWithNewType(Type, newType!);
        }
    }
}