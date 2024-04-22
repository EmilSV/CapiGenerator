using CapiGenerator.CSModel;

namespace CapiGenerator.UtilTypes;

public class CSFullFieldPath(BaseCSType type, ICSFieldLike field)
{
    private readonly BaseCSType _type = type;
    private readonly ICSFieldLike _field = field;

    private uint _typeChangeCount = type.ChangeCount;
    private uint _fieldChangeCount = field.ChangeCount;

    private string? _fullName;

    public string GetFullName()
    {
        if (_fullName == null ||
            _type.ChangeCount != _typeChangeCount ||
            _field.ChangeCount != _fieldChangeCount)
        {
            _typeChangeCount = _type.ChangeCount;
            _fieldChangeCount = _field.ChangeCount;
            if (_type.Namespace != null)
            {
                _fullName = $"{_type.Namespace}.{_type.Name}.{_field.Name}";
            }
            else
            {
                _fullName = $"{_type.Name}.{_field.Name}";
            }
        }
        return _fullName;
    }
}