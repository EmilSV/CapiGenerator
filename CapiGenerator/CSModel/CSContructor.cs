using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;


public class CSConstructor : BaseCSAstItem
{
    public BaseCSType? ParentType { get; private set; }
    public LazyFormatString? Body;

    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<CSParameter> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters = new(null, parameters);
    }

    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<(CSTypeInstance, string)> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters = new(null, ToParameters(parameters));
    }


    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<(ICSType, string)> parameters)
    {
        AccessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters = new(null, ToParameters(parameters));
    }

    public CSAccessModifier AccessModifier;

    public NotifyList<CSParameter> Parameters { get; }

    internal void SetParent(BaseCSType? parent)
    {
        if (ParentType != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }
        ParentType = parent;
    }

    private static CSParameter[] ToParameters(ReadOnlySpan<(CSTypeInstance type, string name)> parameters)
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

    private static CSParameter[] ToParameters(ReadOnlySpan<(ICSType type, string name)> parameters)
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