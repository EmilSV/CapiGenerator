using CapiGenerator.Parser;

namespace CapiGenerator.CModel;


public sealed class CEnum(Guid compilationUnitId, string name, ReadOnlySpan<CEnumField> fields) :
    BaseCAstItem(compilationUnitId), ICType
{
    public string Name => name;
    private readonly CEnumField[] _fields = fields.ToArray();
    public ReadOnlySpan<CEnumField> Fields => _fields;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var value in _fields)
        {
            value.OnSecondPass(compilationUnit);
        }
    }
}