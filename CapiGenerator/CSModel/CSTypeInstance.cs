using System.Text;
using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSTypeInstance : BaseCSAstItem
{
    private readonly ResoleRef<ICSType, ICType> _rRefType;
    private readonly BaseCSTypeModifier[] _modifier;

    public CSTypeInstance(ICType cType, ReadOnlySpan<BaseCSTypeModifier> modifiers = default)
    {
        _rRefType = new(cType);
        _modifier = modifiers.ToArray();
    }

    public CSTypeInstance(ICSType type, ReadOnlySpan<BaseCSTypeModifier> modifiers = default)
    {
        _rRefType = new(type);
        _modifier = modifiers.ToArray();
    }

    public ICSType? Type => _rRefType.Output;
    public IReadOnlyList<BaseCSTypeModifier> Modifiers => _modifier;

    public override void OnSecondPass(CSTranslationUnit compilationUnit)
    {
        _rRefType.TrySetOutputFromResolver(compilationUnit);
        if (_rRefType.Output is { IsAnonymous: true } and ICSSecondPassable anonymousType)
        {
            anonymousType.OnSecondPass(compilationUnit);
        }
    }

    protected static BaseCSTypeModifier[] TranslateModifiers(ReadOnlySpan<CTypeModifier> cModifiers)
    {
        List<BaseCSTypeModifier> modifiers = [];

        foreach (var cModifier in cModifiers)
        {
            BaseCSTypeModifier modifier = cModifier switch
            {
                PointerType => CsPointerType.Instance,
                _ => throw new Exception("unsupported modifier"),
            };
            modifiers.Add(modifier);
        }


        return [.. modifiers];
    }

    public static CSTypeInstance CreateFromCTypeInstance(CTypeInstance cTypeInstance)
    {
        var cType = cTypeInstance.GetCType() ?? throw new Exception("cType is null");
        var modifiers = TranslateModifiers(cTypeInstance.Modifiers);

        if (cType.IsAnonymous)
        {
            return HandleAnonymousType(cType, modifiers);
        }

        var csTypeInstance = new CSTypeInstance(cType, modifiers);
        return csTypeInstance;
    }

    private static CSTypeInstance HandleAnonymousType(ICType cType, ReadOnlySpan<BaseCSTypeModifier> modifiers)
    {
        switch (cType)
        {
            case AnonymousFunctionType functionType:
                var returnType = CreateFromCTypeInstance(functionType.ReturnType);
                var parameters = functionType.Parameters.ToArray().Select(
                    i => CreateFromCTypeInstance(i.GetParameterType())
                ).ToArray();
                var csFunctionType = new CSUnmanagedFunctionType(returnType, parameters);
                if (modifiers is [CsPointerType, ..])
                {
                    return new CSTypeInstance(csFunctionType, modifiers[1..]);
                }
                return new CSTypeInstance(csFunctionType, modifiers);
            default: throw new Exception("unsupported anonymous type");
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(Type switch
        {
            BaseCSType namedType => namedType.GetFullName(),
            BaseCSAnonymousType anonymousType => anonymousType.GetFullTypeDefString(),
            CSPrimitiveType primitiveType => primitiveType.Name,
            _ => throw new Exception("unsupported type")
        });
        foreach (var modifier in _modifier)
        {
            sb.Append(modifier.GetTypeString());
        }

        return sb.ToString();
    }
}