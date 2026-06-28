using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CUnion(
    string name,
    ReadOnlySpan<CField> fields,
    bool isAnonymous = false) :
    BaseCAstItem, ICType
{
    private readonly CField[] _fields = fields.ToArray();

    public string Name => name;
    public ReadOnlySpan<CField> Fields => _fields;
    public bool IsAnonymous => isAnonymous;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var field in Fields)
        {
            field.OnSecondPass(compilationUnit);
        }
    }
}
