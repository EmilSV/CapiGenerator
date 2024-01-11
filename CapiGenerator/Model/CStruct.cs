using CapiGenerator.Parser;

namespace CapiGenerator.Model;

public class CStruct(Guid compilationUnitId, string name, ReadOnlySpan<CField> fields) :
    BaseCAstItem(compilationUnitId), ICType
{
    private readonly CField[] _fields = fields.ToArray();
    public string Name => name;
    public ReadOnlySpan<CField> Fields => _fields;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var field in Fields)
        {
            field.OnSecondPass(compilationUnit);
        }
    }
}