using CapiGenerator.CModel.Type;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;


public class CSUnmanagedFunctionType(
     ReadOnlySpan<CSTypeInstance> parameterTypes,
     CSTypeInstance returnType)
    : BaseCSAstItem()
{
    private readonly CSTypeInstance[] _parameterTypes = parameterTypes.ToArray();
    public ReadOnlySpan<CSTypeInstance> ParameterTypes => _parameterTypes;
    public CSTypeInstance ReturnType => returnType;

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        returnType.OnSecondPass(unit);
        foreach (var parameterType in _parameterTypes)
        {
            parameterType.OnSecondPass(unit);
        }
    }
}