using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSType
    : BaseCSAstItem, ICSType, ICommendableItem, IAttributeAssignableItem, IChildAstItem<BaseCSType>
{
    protected BaseCSType()
    {
    }

    protected BaseCSType(object primarySource)
        : base(primarySource)
    {
    }

    public string? Namespace;
    public required string Name;

    public BaseCSType? Parent { get; private set; }

    public DocComment? Comments { get; set; }
    public List<BaseCSAttribute> Attributes { get; } = [];

    public string GetFullName()
    {
        if (Parent is not null)
        {
            return $"{Parent.GetFullName()}.{Name}";
        }

        if (Namespace is not null)
        {
            return $"{Namespace}.{Name}";
        }

        return Name;
    }

    void IChildAstItem<BaseCSType>.SetParent(BaseCSType? parent)
    {
        if (Parent != null && parent != null)
        {
            throw new InvalidOperationException("Parent type is already set");
        }

        Parent = parent;
    }

    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = Name;
        return true;
    }

    InstanceId ICSType.Id => Id;

    public bool IsAnonymous => false;
}
