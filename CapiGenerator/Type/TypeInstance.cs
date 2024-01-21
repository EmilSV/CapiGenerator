using CapiGenerator.CModel;
using CapiGenerator.Parser;

namespace CapiGenerator.Type;

public class TypeInstance : BaseCAstItem
{
    private object? _cTypeOrTypeName;
    private TypeModifier[]? _modifiers;
    public ICType? CType => _cTypeOrTypeName is ICType cType ? cType : null;
    public string? TypeName => _cTypeOrTypeName is string typeName ? typeName :
        _cTypeOrTypeName is ICType cType ? cType.Name : null;
    public ReadOnlySpan<TypeModifier> Modifiers =>
        _modifiers ?? ReadOnlySpan<TypeModifier>.Empty;

    public bool GetIsCompletedType()
    {
        if (_cTypeOrTypeName is BaseAnonymousType anonymousType)
        {
            return anonymousType.GetIsCompletedType();
        }

        if (_cTypeOrTypeName is ICType)
        {
            return true;
        }

        return false;
    }

    public TypeInstance(Guid compilationUnitId, ICType cType, ReadOnlySpan<TypeModifier> modifiers)
        : base(compilationUnitId)
    {
        _cTypeOrTypeName = cType;
        _modifiers = modifiers.ToArray();
    }

    public TypeInstance(Guid compilationUnitId, string typeName, ReadOnlySpan<TypeModifier> modifiers)
        : base(compilationUnitId)
    {
        _cTypeOrTypeName = typeName;
        _modifiers = modifiers.ToArray();
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        if (compilationUnit.CompilationUnitId != CompilationUnitId)
        {
            throw new InvalidOperationException("Compilation unit id mismatch");
        }

        if (GetIsCompletedType())
        {
            return;
        }

        if (_cTypeOrTypeName is BaseAnonymousType anonymousType)
        {
            anonymousType.OnSecondPass(compilationUnit);
            return;
        }
        
        if(_cTypeOrTypeName is string typeName)
        {
            var newType = compilationUnit.GetTypeByName(typeName);
            if (newType != null)
            {
                _cTypeOrTypeName = newType;
            }
            return;
        }
    }
}
