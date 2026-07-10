using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseCSMemberContainer : BaseCSType, ITypeReplace
{
    protected BaseCSMemberContainer()
    {
        Fields = new(this);
        Methods = new(this);
    }

    protected BaseCSMemberContainer(object primarySource)
        : base(primarySource)
    {
        Fields = new(this);
        Methods = new(this);
    }

    public ChildList<CSField, BaseCSType> Fields { get; }
    public ChildList<CSMethod, BaseCSType> Methods { get; }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var field in Fields)
        {
            field.OnSecondPass(unit);
        }
        foreach (var method in Methods)
        {
            method.OnSecondPass(unit);
        }
    }

    public virtual void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        foreach (var field in Fields)
        {
            field.ReplaceTypes(predicate);
        }
        foreach (var method in Methods)
        {
            method.ReplaceTypes(predicate);
        }
    }
}
