using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSMethod(
    CSTypeInstance returnType,
    string name,
    ReadOnlySpan<CSParameter> parameters
)
    : BaseCSAstItem
{
    private readonly CSParameter[] _parameters = parameters.ToArray();

    public CSTypeInstance ReturnType => returnType;
    public string Name => name;
    public ReadOnlySpan<CSParameter> Parameters => _parameters;

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        returnType.OnSecondPass(unit);
        foreach (var parameter in _parameters)
        {
            parameter.OnSecondPass(unit);
        }
    }
}