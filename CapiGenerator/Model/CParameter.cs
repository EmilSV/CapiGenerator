using CapiGenerator.Parser;

namespace CapiGenerator.Model;

public sealed class CParameter : INeedSecondPass
{
    public readonly string Name;
    private readonly string? _typeName;
    private ICParameterType? _type;

    public ICParameterType? GetParameterType()
    {
        return _type;
    }

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_typeName != null)
        {
            _type = compilationUnit.GetParameterType(_typeName);
        }
    }

    public CParameter(string name, string typeName)
    {
        Name = name;
        _typeName = typeName;
    }

    public CParameter(string name, ICParameterType type)
    {
        Name = name;
        _type = type;
    }
}