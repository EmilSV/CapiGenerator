using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSType
    : BaseCSAstItem, ICSType, ICommendableItem, IAttributeAssignableItem
{
    public string? Namespace;
    public required string Name;

    public DocComment? Comments { get; set; }
    public NotifyList<BaseCSAttribute> Attributes { get; } = new(null);

    public string GetFullName()
    {
        if (Namespace is not null)
        {
            return $"{Namespace}.{Name}";
        }

        return Name;
    }

    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = Name;
        return true;
    }

    InstanceId ICSType.Id => Id;

    public bool IsAnonymous => false;
}