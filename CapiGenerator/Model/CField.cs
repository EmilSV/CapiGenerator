using CapiGenerator.Parser;
using CapiGenerator.Type;

namespace CapiGenerator.Model;

public class CField(
    Guid compilationUnitId, string name, TypeInstance type)
    : BaseCAstItem(compilationUnitId)
{
    public readonly string Name = name;
    private TypeInstance _type = type;

    public TypeInstance GetFieldType()
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