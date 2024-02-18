namespace CapiGenerator.CSModel;


public class CSUnmanagedFunctionType(
     ReadOnlySpan<> parameterTypes)
    : BaseCSAstItem()
{
    private readonly CSResolveType[] _parameterTypes = parameterTypes.ToArray();
    public ReadOnlySpan<CSResolveType> ParameterTypes => _parameterTypes;
    public CSResolveType ReturnType => returnType;
}
