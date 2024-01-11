using CapiGenerator.Parser;

namespace CapiGenerator.Model;


public sealed class CEnum(string name, ReadOnlySpan<CConstant> values) : 
    ICType
{
    public string Name => name;
    private readonly CConstant[] _values = values.ToArray();
    public ReadOnlySpan<CConstant> GetValues() => _values;

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var value in _values)
        {
            value.OnSecondPass(compilationUnit);
        }
    }
}