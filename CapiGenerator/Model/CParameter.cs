using CapiGenerator.Parser;
using CapiGenerator.Type;

namespace CapiGenerator.Model;

public sealed class CParameter(
    Guid compilationUnitId, string name, TypeInstance type) 
    : BaseCAstItem(compilationUnitId)
{
    public readonly string Name = name;
    private TypeInstance _type = type;
    public TypeInstance GetParameterType() => _type;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_type.IsCompletedType)
        {
            return;
        }

        var typeName = _type.TypeName;
        if (typeName != null)
        {
            var newType = compilationUnit.GetTypeByName(typeName);
            if (newType != null)
            {
                _type = _type.NewWithType(newType);
            }
        }
    }
}