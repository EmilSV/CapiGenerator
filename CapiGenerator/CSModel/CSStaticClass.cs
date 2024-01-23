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
}

