using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;

namespace CapiGenerator.CModel;

public class CField(
    Guid compilationUnitId, string name, CTypeInstance type)
    : BaseCAstItem(compilationUnitId)
{
    public readonly string Name = name;
    private CTypeInstance _type = type;

    public CTypeInstance GetFieldType()
    {
        return _type;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_type.GetIsCompletedType())
        {
            return;
        }

        _type.OnSecondPass(compilationUnit);
    }
}