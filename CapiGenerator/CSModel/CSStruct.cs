using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSStruct : BaseCSMemberContainer
{
    public CSStruct()
    {
        Constructors = new(this);
        NestedTypes = new(this);
    }

    public CSStruct(object primarySource)
        : base(primarySource)
    {
        Constructors = new(this);
        NestedTypes = new(this);
    }

    public CSAccessModifier AccessModifier;
    public bool IsUnsafe;
    public bool IsPartial;
    public bool IsReadOnly;

    public HashSet<LazyFormatString> Interfaces { get; } = [];
    public ChildList<CSConstructor, BaseCSType> Constructors { get; }
    public ChildList<BaseCSType, BaseCSType> NestedTypes { get; }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        base.OnSecondPass(unit);
        foreach (var constructor in Constructors)
        {
            constructor.OnSecondPass(unit);
        }
        foreach (var nestedType in NestedTypes)
        {
            nestedType.OnSecondPass(unit);
        }
    }

    public override void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        base.ReplaceTypes(predicate);
        foreach (var constructor in Constructors)
        {
            constructor.ReplaceTypes(predicate);
        }
        foreach (var nestedType in NestedTypes)
        {
            if (nestedType is ITypeReplace typeReplace)
            {
                typeReplace.ReplaceTypes(predicate);
            }
        }
    }
}
