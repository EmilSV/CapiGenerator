using CapiGenerator.Model;

namespace CapiGenerator.Type;

public struct TypeInstance
{
    private object? _cTypeOrTypeName;
    private TypeModifier[]? _modifiers;
    public readonly ICType? CType => _cTypeOrTypeName is ICType cType ? cType : null;
    public readonly string? TypeName => _cTypeOrTypeName is string typeName ? typeName :
        _cTypeOrTypeName is ICType cType ? cType.Name : null;
    public readonly ReadOnlySpan<TypeModifier> Modifiers =>
        _modifiers ?? ReadOnlySpan<TypeModifier>.Empty;

    public readonly bool IsCompletedType => _cTypeOrTypeName is ICType;

    public TypeInstance(ICType cType, ReadOnlySpan<TypeModifier> modifiers)
    {
        _cTypeOrTypeName = cType;
        _modifiers = modifiers.ToArray();
    }

    public TypeInstance(string typeName, ReadOnlySpan<TypeModifier> modifiers)
    {
        _cTypeOrTypeName = typeName;
        _modifiers = modifiers.ToArray();
    }

    public readonly TypeInstance NewWithType(ICType cType)
    {
        TypeInstance newTypeInstance = default;
        newTypeInstance._cTypeOrTypeName = cType;
        newTypeInstance._modifiers = _modifiers;

        return newTypeInstance;
    }

}
