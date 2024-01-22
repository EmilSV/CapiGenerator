namespace CapiGenerator.CSModel;

public class CSMethod(
    string name,
    CSResolveType returnType,
    ReadOnlySpan<CSParameter> parameters
)
{
    private readonly CSParameter[] _parameters = parameters.ToArray();

    public string Name => name;
    public CSResolveType ReturnType => returnType;
    public ReadOnlySpan<CSParameter> Parameters => _parameters;
}