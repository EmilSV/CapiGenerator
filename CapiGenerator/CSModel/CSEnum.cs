using CapiGenerator.CModel;
using CapiGenerator.UtilTypes;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSEnum : BaseCSType,
    INotifyReviver<CSEnumField>, ITypeReplace
{
    public CSPrimitiveType Type = CSPrimitiveType.Instances.Int;

    public CSEnum()
    {
        Values = new(this);
    }

    public NotifyUniqueList<CSEnumField> Values { get; private set; }

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

    void INotifyReviver<CSEnumField>.OnAddRange(ReadOnlySpan<CSEnumField> items)
    {
        foreach (var item in items)
        {
            item.SetParent(this);
        }
    }

    void INotifyReviver<CSEnumField>.OnRemoveRange(ReadOnlySpan<CSEnumField> items)
    {
        foreach (var item in items)
        {
            item.SetParent(null);
        }
    }
}