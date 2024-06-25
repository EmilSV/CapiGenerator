using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;


public class CSConstructor : BaseCSAstItem
{
    private CSAccessModifier _accessModifier = CSAccessModifier.Public;
    private LazyFormatString? _body;

    public BaseCSType? ParentType { get; private set; }
    public LazyFormatString? Body
    {
        get => _body;
        set
        {
            if (_body != value)
            {
                _body = value;
                NotifyChange();
            }
        }
    }

    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<CSParameter> parameters)
    {
        _accessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters = new(null, parameters);
    }

    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<(CSTypeInstance, string)> parameters)
    {
        _accessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters = new(null, ToParameters(parameters));
    }


    public CSConstructor(CSClassMemberModifier modifiers, ReadOnlySpan<(ICSType, string)> parameters)
    {
        _accessModifier = CSAccessModifierHelper.GetAccessModifier(modifiers);
        Parameters = new(null, ToParameters(parameters));
    }

    public CSAccessModifier AccessModifier
    {
        get => _accessModifier;
        set
        {
            if (_accessModifier != value)
            {
                _accessModifier = value;
                NotifyChange();
            }
        }
    }

    public NotifyList<CSParameter> Parameters { get; }

    internal void SetParent(BaseCSType? parent)
    {
        if (ParentType != null && parent != null)
        {
            throw new InvalidOperationException("Parent method is already set");
        }
        ParentType = parent;
        NotifyChange();
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