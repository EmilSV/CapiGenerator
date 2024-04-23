using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;


public abstract class BaseCSType
    : BaseCSAstItem, ICSType
{
    private string? _namespace;
    private string? _name;
    public string? Namespace
    {
        get => _namespace;
        set
        {
            if (_namespace != value)
            {
                _namespace = value;
                NotifyChange();
            }
        }
    }
    public required string Name
    {
        get => _name!;
        set
        {
            if (_name != value)
            {
                _name = value;
                NotifyChange();
            }
        }
    }

    public string GetFullName()
    {
        if (Namespace is not null)
        {
            return $"{Namespace}.{Name}";
        }

        return Name;
    }

    InstanceId ICSType.Id => Id;

    public bool IsAnonymous => false;
}