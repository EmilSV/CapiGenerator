using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSStruct : CSBaseType
{
    private readonly HistoricList<CSField> _fields;
    private readonly HistoricList<CSMethod> _methods;
    private readonly ComputedValue<string> _fullName;


    public CSStruct(string name, ReadOnlySpan<CSField> fields, ReadOnlySpan<CSMethod> methods)
        : base(name)
    {
        _fields = new(fields);
        _methods = new(methods);
        foreach (var field in _fields)
        {
            field.SetParent(this);
        }
        foreach (var method in _methods)
        {
            method.SetParent(this);
        }

        _fullName = new ComputedValue<string>(
            dependencies: [Namespace, Name],
            compute: () => Namespace != null ? $"{Namespace.Value}.{Name.Value}" : Name.Value!
        );
    }


    public HistoricList<CSField> Fields => _fields;
    public HistoricList<CSMethod> Methods => _methods;

    public HistoricValue<string?> Namespace { get; } = new();
    public override ComputedValueOrValue<string> FullName => _fullName;


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

