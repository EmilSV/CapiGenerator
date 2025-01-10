using CapiGenerator.Translator;
using CapiGenerator.CSModel.Comments;

namespace CapiGenerator.CSModel;

public class CSEnumField : BaseCSAstItem, ICSFieldLike
{
    public CSEnum? ParentEnum { get; private set; }
    public required string Name { get; set; }
    public CSConstantExpression Expression = [];

    public DocComment? Comments { get; set; }

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