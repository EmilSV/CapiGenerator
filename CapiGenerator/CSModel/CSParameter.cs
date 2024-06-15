using CapiGenerator.CModel;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSParameter(
    CSTypeInstance type, string name, CSDefaultValue defaultValue = default
) : BaseCSAstItem
{
    public string Name => name;
    public CSTypeInstance Type => type;
    public CSDefaultValue DefaultValue => defaultValue;
    public CSMethod? ParentMethod { get; private set; }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        type.OnSecondPass(unit);
        defaultValue.OnSecondPass(unit);
    }

    internal void SetParentMethod(CSMethod? parent)
    {
        if (ParentMethod != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }

        ParentMethod = parent;
    }

    public static CSParameter FromCParameter(CParameter parameter) => new(
        type: CSTypeInstance.CreateFromCTypeInstance(parameter.GetParameterType()),
        name: parameter.Name,
        defaultValue: CSDefaultValue.NullValue
    );

    public static CSParameter CopyWithNewType(CSParameter original, ICSType newType)
    {
        return new CSParameter(CSTypeInstance.CopyWithNewType(original.Type, newType), original.Name, original.DefaultValue);
    }

    public static CSParameter[] EmptyParameters => [];
}