using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel.EnrichData;


namespace CapiGenerator.CSModel;

public class CSResolveType
{
    private readonly CTypeInstance _cTypeInstance;
    private readonly CSBaseType? _resolvedType;
    private readonly BaseCSTypeResolver? _resolver;


    public CSResolveType(CTypeInstance cTypeInstance, BaseCSTypeResolver resolver)
    {
        _cTypeInstance = cTypeInstance;
        _resolver = resolver;
    }

    public CSResolveType(CTypeInstance typeInstance, CSBaseType resolvedType)
    {
        _cTypeInstance = typeInstance;
        _resolvedType = resolvedType;
    }


    public CTypeInstance CTypeInstance => _cTypeInstance;
    public bool GetIsResolved()
    {
        if (_resolvedType != null)
        {
            return true;
        }

        return _cTypeInstance.CType != null && _resolver!.IsResolved(_cTypeInstance.CType!);
    }

    public CSBaseType? GetResolvedType()
    {
        return typeInstance.CType is BaseCAstItem baseCAstItem ?
            baseCAstItem?.EnrichingDataStore.Get<CSTranslationsTypeData>()?.CsType :
            null;
    }
}