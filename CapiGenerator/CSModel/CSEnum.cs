using CapiGenerator.CModel;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSEnum : BaseCSType, ITypeReplace
{
    public CSEnum()
    {
        Values = new(this);
    }

    public CSEnum(object primarySource)
        : base(primarySource)
    {
        Values = new(this);
    }

    public CSAccessModifier AccessModifier = CSAccessModifier.Public;
    public CSPrimitiveType Type = CSPrimitiveType.Instances.Int;

    public readonly ChildList<CSEnumField, CSEnum> Values;

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var value in Values)
        {
            value.OnSecondPass(unit);
        }
    }

    public void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        if (predicate(Type, out var newType))
        {
            if (newType is CSPrimitiveType primitiveType)
            {
                Type = primitiveType;
            }
            else
            {
                Console.Error.WriteLine($"Type {newType} is not supported for enum");
            }
        }
    }
}
