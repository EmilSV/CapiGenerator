using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CapiGenerator.Translator;

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
    public ReadOnlySpan<BaseCSTypeModifier> Modifiers => _modifier;
    public ResoleRef<ICSType, ICType> RRefType => _rRefType;

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
        var csTypeInstance = new CSTypeInstance(cType, TranslateModifiers(cTypeInstance.Modifiers));
        return csTypeInstance;
    }
}