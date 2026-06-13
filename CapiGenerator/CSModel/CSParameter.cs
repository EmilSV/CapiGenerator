using CapiGenerator.CModel;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSParameter(
    CSTypeInstance type, string name, CSDefaultValue defaultValue = default
) : BaseCSAstItem, IChildAstItem<BaseCSCallableType>
{
    public string Name => name;
    public CSTypeInstance Type => type;
    public CSDefaultValue DefaultValue => defaultValue;
    public BaseCSCallableType? Parent { get; private set; }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        type.OnSecondPass(unit);
        defaultValue.OnSecondPass(unit);
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

    void IChildAstItem<BaseCSCallableType>.SetParent(BaseCSCallableType? parent)
    {
        if (Parent != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }

        Parent = parent;
    }

    public static CSParameter[] EmptyParameters => [];

    public static CSParameter[] ParameterArrayFromTurples(ReadOnlySpan<(CSTypeInstance, string)> parameters)
    {
        if (parameters.Length == 0)
        {
            return [];
        }

        var array = new CSParameter[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var (type, name) = parameters[i];
            array[i] = new(type, name);
        }
        return array;
    }

    public static CSParameter[] ParameterArrayFromTurples(ReadOnlySpan<(ICSType type, string name)> parameters)
    {
        if (parameters.Length == 0)
        {
            return [];
        }

        var array = new CSParameter[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            var (type, name) = parameters[i];
            array[i] = new(new(type), name);
        }
        return array;
    }
}
