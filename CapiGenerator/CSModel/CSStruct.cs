using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSStruct : CSBaseType
{
    public CSStruct(string name, ReadOnlySpan<CSField> fields, ReadOnlySpan<CSMethod> methods)
        : base(name)
    {
        _fields = fields.ToArray();
        _methods = methods.ToArray();
        foreach (var field in _fields)
        {
            field.SetParent(this);
        }
        foreach (var method in _methods)
        {
            method.SetParent(this);
        }
    }

    private readonly CSField[] _fields;
    private readonly CSMethod[] _methods;
    private string? _fullName;

    public ReadOnlySpan<CSField> Fields => _fields;
    public ReadOnlySpan<CSMethod> Methods => _methods;

    public string? Namespace { get; init; }
    public string FullName => _fullName ??= Namespace is null ? Name : $"{Namespace}.{Name}";

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var field in _fields)
        {
            field.OnSecondPass(unit);
        }
        foreach (var method in _methods)
        {
            method.OnSecondPass(unit);
        }
    }
}

