using CapiGenerator.CModel;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSParameter : BaseCSAstItem, IChildAstItem<BaseCSCallableType>
{
    public CSParameter(CSTypeInstance type, string name, CSDefaultValue defaultValue = default)
    {
        Type = type;
        Name = name;
        DefaultValue = defaultValue;
    }

    public CSParameter(object primarySource, CSTypeInstance type, string name, CSDefaultValue defaultValue = default)
        : base(primarySource)
    {
        Type = type;
        Name = name;
        DefaultValue = defaultValue;
    }

    public string Name { get; }
    public CSTypeInstance Type { get; }
    public CSDefaultValue DefaultValue { get; }
    public BaseCSCallableType? Parent { get; private set; }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        Type.OnSecondPass(unit);
        DefaultValue.OnSecondPass(unit);
    }

    public static CSParameter FromCParameter(CParameter parameter) => new(
        primarySource: parameter,
        type: CSTypeInstance.CreateFromCTypeInstance(parameter.GetParameterType()),
        name: parameter.Name,
        defaultValue: CSDefaultValue.NullValue
    );

    public static CSParameter CopyWithNewType(CSParameter original, ICSType newType)
    {
        var parameter = original.PrimarySource is null
            ? new CSParameter(CSTypeInstance.CopyWithNewType(original.Type, newType), original.Name, original.DefaultValue)
            : new CSParameter(original.PrimarySource, CSTypeInstance.CopyWithNewType(original.Type, newType), original.Name, original.DefaultValue);

        parameter.AddSecondarySources(original.SecondarySources);
        return parameter;
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
