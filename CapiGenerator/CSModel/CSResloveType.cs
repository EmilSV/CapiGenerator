using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel.EnrichData;


namespace CapiGenerator.CSModel;

public record CSResolveType(
    CTypeInstance typeInstance
)
{
    public CTypeInstance TypeInstance => typeInstance;
    public bool GetIsResolved()
    {
        return typeInstance.CType is BaseCAstItem baseCAstItem && baseCAstItem.GetEnrichingData<CSTranslationsTypeData>() != null;
    }

    public CSBaseType? GetResolvedType()
    {
        return typeInstance.CType is BaseCAstItem baseCAstItem ?
            baseCAstItem?.GetEnrichingData<CSTranslationsTypeData>()?.CsType :
            null;
    }
}