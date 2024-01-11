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
        if (_type.IsCompletedType)
        {
            return;
        }

        var _typeName = _type.TypeName ??
                throw new InvalidOperationException("Field type name is null");

        var foundType = compilationUnit.GetTypeByName(_typeName) ??
             throw new InvalidOperationException($"Field type '{_typeName}' not found");

        _type = _type.NewWithType(foundType);
    }
}