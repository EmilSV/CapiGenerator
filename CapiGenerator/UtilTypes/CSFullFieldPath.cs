using CapiGenerator.CSModel;

namespace CapiGenerator.UtilTypes;

public readonly struct CSFullFieldPath(BaseCSType type, ICSFieldLike field)
{
    private readonly BaseCSType _type = type;
    private readonly ICSFieldLike _field = field;

    public string GetFullName()
    {
        if (_type.Namespace != null)
        {
            return $"{_type.Namespace}.{_type.Name}.{_field.Name}";
        }
        else
        {
            return $"{_type.Name}.{_field.Name}";
        }
    }
}