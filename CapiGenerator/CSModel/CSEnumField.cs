using CapiGenerator.Translator;
using CapiGenerator.CSModel.Comments;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSEnumField :
    BaseCSAstItem, ICSFieldLike, ICommendableItem,
    IAttributeAssignableItem, IChildAstItem<CSEnum>
{
    public CSEnum? Parent { get; private set; }
    public required string Name { get; set; }
    public CSConstantExpression Expression = [];

    public DocComment? Comments { get; set; }
    public List<BaseCSAttribute> Attributes { get; } = [];

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var expression in Expression)
        {
            expression.OnSecondPass(unit);
        }
    }

    void IChildAstItem<CSEnum>.SetParent(CSEnum? parent)
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
            throw new InvalidOperationException("Parent enum is not set");
        }

        return $"{Parent.GetFullName()}.{Name}";
    }
}
