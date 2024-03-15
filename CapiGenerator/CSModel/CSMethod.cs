using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSMethod(
    CSTypeInstance returnType,
    string name,
    ReadOnlySpan<CSParameter> parameters,
    string? body = null
)
    : BaseCSAstItem
{
    private readonly CSParameter[] _parameters = parameters.ToArray();

    public CSTypeInstance ReturnType => returnType;
    public string Name => name;
    public ReadOnlySpan<CSParameter> Parameters => _parameters;
    public string? Body => body;

    public bool IsStatic { get; init; }
    public bool IsExtern { get; init; }
    public CSAccessModifier AccessModifier { get; init; } = CSAccessModifier.Public;

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        returnType.OnSecondPass(unit);
        foreach (var parameter in _parameters)
        {
            parameter.OnSecondPass(unit);
        }
    }
}