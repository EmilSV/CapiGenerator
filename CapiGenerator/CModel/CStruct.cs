using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CStruct(string name, ReadOnlySpan<CField> fields) :
    BaseCAstItem, ICType
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