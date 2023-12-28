using CapiGenerator.Parser;

namespace CapiGenerator.Model;

public class CField : INeedSecondPass
{
    public readonly string Name;
    private readonly string? _typeName;
    private ICFieldType? _type;

    public ICFieldType? GetFieldType()
    {
        return _type;
    }

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_typeName != null)
        {
            _type = compilationUnit.GetFieldType(_typeName);
        }
    }

    public CField(string name, string typeName)
    {
        Name = name;
        _typeName = typeName;
    }

    public CField(string name, ICFieldType type)
    {
        Name = name;
        _type = type;
    }
}