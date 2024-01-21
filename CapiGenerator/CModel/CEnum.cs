using CapiGenerator.Parser;

namespace CapiGenerator.CModel;


public sealed class CEnum(Guid compilationUnitId, string name, ReadOnlySpan<CConstant> values) :
    BaseCAstItem(compilationUnitId), ICType
{
    public string Name => name;
    private readonly CConstant[] _values = values.ToArray();
    public ReadOnlySpan<CConstant> GetValues() => _values;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var value in _values)
        {
            value.OnSecondPass(compilationUnit);
        }
    }
}