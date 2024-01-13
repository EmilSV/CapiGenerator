using CapiGenerator.Parser;
using CapiGenerator.Type;

namespace CapiGenerator.Model;

public class CTypedef(Guid compilationUnitId, string name, TypeInstance innerType) :
    BaseCAstItem(compilationUnitId), ICType
{
    private TypeInstance _innerType = innerType;
    public string Name => name;
    public TypeInstance InnerType => _innerType;

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (_innerType.IsCompletedType)
        {
            return;
        }

        var _typeName = _innerType.TypeName ??
                throw new InvalidOperationException("typedef inner type name is null");

        var foundType = compilationUnit.GetTypeByName(_typeName) ??
             throw new InvalidOperationException($"typedef inner : '{_typeName}' not found");

        _innerType = _innerType.NewWithType(foundType);
    }
}