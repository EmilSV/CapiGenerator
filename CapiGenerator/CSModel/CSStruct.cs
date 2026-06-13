using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSStruct : BaseCSType, ITypeReplace
{
    public CSStruct()
    {
        Fields = new(this);
        Methods = new(this);
        Constructors = new(this);
    }

    public CSAccessModifier AccessModifier;
    public bool IsUnsafe;
    public bool IsPartial;
    public bool IsReadOnly;

    public HashSet<LazyFormatString> Interfaces { get; } = [];
    public ChildList<CSField, BaseCSType> Fields { get; }
    public ChildList<CSMethod, BaseCSType> Methods { get; }
    public ChildList<CSConstructor, BaseCSType> Constructors { get; }

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
