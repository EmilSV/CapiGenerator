using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSTypeInstance : BaseCSAstItem
{
    private readonly HistoricResoleRef<ICSType, ICType> _rRefType;
    private readonly HistoricList<BaseCSTypeModifier> _modifier;

    public CSTypeInstance(ICType cType, ReadOnlySpan<BaseCSTypeModifier> modifiers = default)
    {
        _rRefType = new(cType);
        _modifier = new(modifiers);
    }

    public CSTypeInstance(ICSType type, ReadOnlySpan<BaseCSTypeModifier> modifiers = default)
    {
        _rRefType = new(type);
        _modifier = new(modifiers);
    }

    public ICSType? Type => _rRefType.Output;
    public HistoricList<BaseCSTypeModifier> Modifiers => _modifier;
    public HistoricResoleRef<ICSType, ICType> RRefType => _rRefType;

    public override void OnSecondPass(CSTranslationUnit compilationUnit)
    {
        _rRefType.TrySetOutputFromResolver(compilationUnit);
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
                return new CSTypeInstance(csFunctionType, modifiers);
            default: throw new Exception("unsupported anonymous type");
        }
    }
}