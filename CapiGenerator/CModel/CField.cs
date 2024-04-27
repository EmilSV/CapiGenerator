using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel;

public class CField(string name, CTypeInstance type)
    : BaseCAstItem
{
    public readonly string Name = name;
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