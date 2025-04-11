using CapiGenerator.Translator;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSEnumField : BaseCSAstItem, ICSFieldLike, ICommendableItem, IAttributeAssignableItem
{
    public CSEnum? ParentEnum { get; private set; }
    public required string Name { get; set; }
    public CSConstantExpression Expression = [];

    public DocComment? Comments { get; set; }
    public NotifyList<BaseCSAttribute> Attributes { get; } = new(null);

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var expression in Expression)
        {
            expression.OnSecondPass(unit);
        }
    }

    internal void SetParent(CSEnum? parent)
    {
        if (ParentEnum != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }
        ParentEnum = parent;
    }

    public string GetFullName()
    {
        if (ParentEnum == null)
        {
            throw new InvalidOperationException("Parent enum is not set");
        }

        return $"{ParentEnum.GetFullName()}.{Name}";
    }
}