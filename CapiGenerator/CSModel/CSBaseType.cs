using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;


public abstract class CSBaseType
    : BaseCSAstItem, ICSType
{
    private volatile uint _changeCounter = 0;
    private string? _namespace;
    private string? _name;

    public readonly InstanceId Id = new();
    InstanceId ICSType.Id => Id;
    public uint ChangeCounter => _changeCounter;

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


    protected void NotifyChange()
    {
        _changeCounter++;
    }
}