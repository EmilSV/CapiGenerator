using CapiGenerator.CModel;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public class CSField : BaseCSAstItem
{
    private readonly string _name;
    private readonly ResoleRef<ICSType, ICType> _rRefType;
    private readonly CSDefaultValue? _defaultValue;

    public CSField(string name, ICSType csType, CSDefaultValue? defaultValue = default)
    {
        _name = name;
        _rRefType = new(csType);
        _defaultValue = defaultValue;
    }

    public CSField(string name, ICType cType, CSDefaultValue? defaultValue = default)
    {
        _name = name;
        _rRefType = new(cType);
        _defaultValue = defaultValue;
    }


    public string Name => _name;
    public ResoleRef<ICSType, ICType> RRefType => _rRefType;
    public ICSType? Type => _rRefType.Output;
    public CSDefaultValue? DefaultValue => _defaultValue;

    public bool IsConst { get; init; }
    public bool IsStatic { get; init; }
    public bool IsReadOnly { get; init; }
    public CSAccessModifier AccessModifier { get; init; } = CSAccessModifier.Public;

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        _rRefType.TrySetOutputFromResolver(unit);
    }
}