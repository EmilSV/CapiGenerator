using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSStruct : BaseCSMemberContainer
{
    public CSStruct()
    {
        Constructors = new(this);
    }

    public CSAccessModifier AccessModifier;
    public bool IsUnsafe;
    public bool IsPartial;
    public bool IsReadOnly;

    public HashSet<LazyFormatString> Interfaces { get; } = [];
    public ChildList<CSConstructor, BaseCSType> Constructors { get; }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        base.OnSecondPass(unit);
        foreach (var constructor in Constructors)
        {
            constructor.OnSecondPass(unit);
        }
    }

    public override void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        base.ReplaceTypes(predicate);
        foreach (var constructor in Constructors)
        {
            constructor.ReplaceTypes(predicate);
        }
    }
}
