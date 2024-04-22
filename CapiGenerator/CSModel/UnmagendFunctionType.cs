using System.Text;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;


public class CSUnmanagedFunctionType(
     CSTypeInstance returnType,
     ReadOnlySpan<CSTypeInstance> parameterTypes)
    : BaseCSAnonymousType()
{
    private readonly CSTypeInstance[] _parameterTypes = parameterTypes.ToArray();
    public ReadOnlySpan<CSTypeInstance> ParameterTypes => _parameterTypes;
    public CSTypeInstance ReturnType => returnType;

    public override string GetFullTypeDefString()
    {
        var sb = new StringBuilder();
        sb.Append("delegate* unmanaged[Cdecl]<");
        foreach (var parameterType in _parameterTypes)
        {
            sb.Append(parameterType.ToString());
            sb.Append(", ");
        }
        sb.Append(returnType.ToString());
        sb.Append('>');
        return sb.ToString();
    }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        returnType.OnSecondPass(unit);
        foreach (var parameterType in _parameterTypes)
        {
            parameterType.OnSecondPass(unit);
        }
    }
}
