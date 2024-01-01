using CapiGenerator.Parser;

namespace CapiGenerator.Model;


public sealed class CEnum(string name, ReadOnlySpan<CConstant> values) : INeedSecondPass
{
    public readonly string Name = name;
    private readonly CConstant[] _values = values.ToArray();

    public IReadOnlyList<CConstant> GetValues() => _values;
    public ReadOnlySpan<CConstant> GetValuesAsSpan() => _values;

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var value in _values)
        {
            value.OnSecondPass(compilationUnit);
        }
    }
}