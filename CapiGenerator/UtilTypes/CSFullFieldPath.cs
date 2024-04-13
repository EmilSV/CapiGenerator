using CapiGenerator.CSModel;

namespace CapiGenerator.UtilTypes;

public class CSFullFieldPath
{
    private readonly CSBaseType _type;
    private readonly CSField _field;

    private uint _typeChangeCount;
    private uint _fieldChangeCount;

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


    public CSFullFieldPath(CSStaticClass type, CSField field)
    {
        _type = type;
        _field = field;
        _typeChangeCount = type.ChangeCount;
        _fieldChangeCount = field.ChangeCount;
    }
}