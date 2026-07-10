using System.Text;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;


public class CSUnmanagedFunctionType(
     object primarySource,
     CSTypeInstance returnType,
     ReadOnlySpan<CSTypeInstance> parameterTypes)
    : BaseCSAnonymousType(primarySource)
{
    private CSTypeInstance _returnType = returnType;
    private readonly CSTypeInstance[] _parameterTypes = parameterTypes.ToArray();
    public ReadOnlySpan<CSTypeInstance> ParameterTypes => _parameterTypes;
    public CSTypeInstance ReturnType => _returnType;

    public override string GetFullTypeDefString()
    {
        var sb = new StringBuilder();
        sb.Append("delegate* unmanaged[Cdecl]<");
        foreach (var parameterType in _parameterTypes)
        {
            sb.Append(parameterType.ToString());
            sb.Append(", ");
        }
        sb.Append(_returnType.ToString());
        sb.Append('>');
        return sb.ToString();
    }

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        _returnType.OnSecondPass(unit);
        foreach (var parameterType in _parameterTypes)
        {
            parameterType.OnSecondPass(unit);
        }
    }

    public void ReplaceTypeInstances(Func<CSTypeInstance, CSTypeInstance> replaceTypeInstance)
    {
        _returnType = replaceTypeInstance(_returnType);
        for (int i = 0; i < _parameterTypes.Length; i++)
        {
            _parameterTypes[i] = replaceTypeInstance(_parameterTypes[i]);
        }
    }

    public override void ReplaceTypes(ITypeReplace.ReplacePredicate predicate)
    {
        for (int i = 0; i < _parameterTypes.Length; i++)
        {
            if (predicate(_parameterTypes[i].Type!, out var newParameterType))
            {
                _parameterTypes[i] = CSTypeInstance.CopyWithNewType(_parameterTypes[i], newParameterType!);
            }
        }

        if (predicate(_returnType.Type!, out var newType))
        {
            _returnType = CSTypeInstance.CopyWithNewType(_returnType, newType!);
        }
    }
}
