using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;


namespace CapiGenerator.CSModel;

public class CSResolveType
{
    private readonly object _cType;
    private CSBaseType? _resolvedType;
    private readonly BaseCSTypeResolver? _resolver;
    private readonly CSBaseTypeModifier[]? _modifiers;


    public CSResolveType(
            CTypeInstance cTypeInstance,
            BaseCSTypeResolver resolver,
            CSBaseTypeModifier[]? modifiers = null)
    {
        _cType = cTypeInstance;
        _resolver = resolver;
        _modifiers = modifiers ?? TranslateModifiers(cTypeInstance.Modifiers);
    }

    public CSResolveType(
        CTypeInstance cTypeInstance,
        CSBaseType resolvedType,
        CSBaseTypeModifier[]? modifiers = null)
    {
        _cType = cTypeInstance;
        _resolvedType = resolvedType;
        _modifiers = modifiers ?? TranslateModifiers(cTypeInstance.Modifiers);
    }

    public CSResolveType(
        ICType cType,
        CSBaseType resolvedType,
        CSBaseTypeModifier[]? modifiers = null)
    {
        _cType = cType;
        _resolvedType = resolvedType;
        _modifiers = modifiers;
    }

    private static CSBaseTypeModifier[] TranslateModifiers(ReadOnlySpan<CTypeModifier> modifiers)
    {
        var result = new CSBaseTypeModifier[modifiers.Length];
        for (var i = 0; i < modifiers.Length; i++)
        {
            result[i] = TranslateModifier(modifiers[i]);
        }

        return result;
    }

    private static CSBaseTypeModifier TranslateModifier(CTypeModifier modifier) => modifier switch
    {
        PointerType or ArrayType => CsPointerType.Instance,
        _ => throw new ArgumentOutOfRangeException(nameof(modifier))
    };


    public ReadOnlySpan<CSBaseTypeModifier> GetModifiers()
    {
        if (_modifiers == null)
        {
            return ReadOnlySpan<CSBaseTypeModifier>.Empty;
        }

        return _modifiers;
    }


    public CTypeInstance? CTypeInstance => _cType is CTypeInstance cTypeInstance ? cTypeInstance : null;
    public ICType? CType => _cType is ICType cType ? cType : null;


    public bool GetIsResolved()
    {
        if (_resolvedType != null)
        {
            return true;
        }

        if (_cType is CTypeInstance cTypeInstance)
        {
            return cTypeInstance.CType != null && _resolver!.IsResolved(cTypeInstance.CType);
        }
        else if (_cType is ICType cType)
        {
            return _resolver!.IsResolved(cType);
        }

        return false;
    }

    public CSBaseType? GetResolvedType()
    {
        if (_resolvedType != null)
        {
            return _resolvedType;
        }

        if (_cType is CTypeInstance cTypeInstance && cTypeInstance.CType != null)
        {
            _resolvedType = _resolver!.Resolve(cTypeInstance.CType);
        }
        else if (_cType is ICType cType)
        {
            _resolvedType = _resolver!.Resolve(cType);
        }

        return _resolvedType;
    }
}