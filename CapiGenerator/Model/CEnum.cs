using CapiGenerator.Parser;

namespace CapiGenerator.Model;


public sealed class CEnum(string name, ReadOnlySpan<CConst> values) : INeedSecondPass
{
    public readonly string Name = name;
    private readonly CConst[] _values = values.ToArray();

    public IReadOnlyList<CConst> GetValues() => _values;
    public ReadOnlySpan<CConst> GetValuesAsSpan() => _values;

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var value in _values)
        {
            value.OnSecondPass(compilationUnit);
        }
    }
}