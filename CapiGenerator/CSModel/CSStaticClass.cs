using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSStaticClass(
    string name,
    ReadOnlySpan<CSField> fields,
    ReadOnlySpan<CSMethod> methods
) : CSBaseType(name)
{
    private readonly CSField[] _fields = fields.ToArray();
    private readonly CSMethod[] _methods = methods.ToArray();

    public ReadOnlySpan<CSField> Fields => _fields;
    public ReadOnlySpan<CSMethod> Methods => _methods;

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

