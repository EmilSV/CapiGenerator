using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;


public abstract class CSBaseType 
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

    InstanceId ICSType.Id => Id;
}