namespace CapiGenerator.CSModel;


public class CSUnmanagedFunctionType(
    ReadOnlySpan<CSResolveType> parameterTypes, CSResolveType returnType)
    : BaseCSAstItem()
{
    private readonly CSResolveType[] _parameterTypes = parameterTypes.ToArray();
    public ReadOnlySpan<CSResolveType> ParameterTypes => _parameterTypes;
    public CSResolveType ReturnType => returnType;
}
