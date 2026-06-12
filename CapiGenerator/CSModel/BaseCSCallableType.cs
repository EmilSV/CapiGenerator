
namespace CapiGenerator.CSModel;

public class BaseCSCallableType : BaseCSAstItem
{
    public IReadOnlyList<CSParameter> Parameters => _parameters;
    private readonly List<CSParameter> _parameters = new();


}
