using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CUnion(
    string name,
    ReadOnlySpan<CField> fields,
    int? sizeOf = null,
    int? alignOf = null) :
    BaseCAstItem, ICType
{
    private readonly CField[] _fields = fields.ToArray();

    public string Name => name;
    public ReadOnlySpan<CField> Fields => _fields;
    public int? SizeOf => sizeOf;
    public int? AlignOf => alignOf;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var field in Fields)
        {
            field.OnSecondPass(compilationUnit);
        }
    }
}
