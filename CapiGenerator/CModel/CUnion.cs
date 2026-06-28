using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public class CUnion(
    string name,
    ReadOnlySpan<CField> fields,
    bool isAnonymous = false,
    ReadOnlySpan<ICType> nestedTypes = default) :
    BaseCAstItem, ICType
{
    private readonly CField[] _fields = fields.ToArray();
    private readonly ICType[] _nestedTypes = nestedTypes.ToArray();

    public string Name => name;
    public ReadOnlySpan<CField> Fields => _fields;
    public bool IsAnonymous => isAnonymous;
    public ReadOnlySpan<ICType> NestedTypes => _nestedTypes;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var field in Fields)
        {
            field.OnSecondPass(compilationUnit);
        }

        foreach (var nestedType in NestedTypes)
        {
            if (nestedType is ICSecondPassable secondPassable)
            {
                secondPassable.OnSecondPass(compilationUnit);
            }
        }
    }
}
