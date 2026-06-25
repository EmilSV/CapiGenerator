using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel;

public class CField(string name, CTypeInstance type, int offset = 0)
    : BaseCAstItem
{
    public readonly string Name = name;
    public readonly int Offset = offset;
    private CTypeInstance _type = type;

    public CTypeInstance GetFieldType()
    {
        return _type;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        _type.OnSecondPass(compilationUnit);
    }
}
