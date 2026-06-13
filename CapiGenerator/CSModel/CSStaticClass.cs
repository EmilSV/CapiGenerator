using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSStaticClass : BaseCSType, ITypeReplace
{
    public CSStaticClass()
    {
        Fields = new(this);
        Methods = new(this);
    }

    public bool IsPartial;

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

    public void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
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
