using CapiGenerator.CModel;
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
}