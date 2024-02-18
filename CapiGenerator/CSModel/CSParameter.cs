using CapiGenerator.CModel;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSParameter : BaseCSAstItem
{
    private readonly string _name;
    private readonly ResoleRef<ICSType, ICType> _rRefType;
    private readonly CSDefaultValue _defaultValue;

    public CSParameter(string name, ICType cType, CSDefaultValue defaultValue = default)
    {
        _name = name;
        _rRefType = new(cType);
        _defaultValue = defaultValue;
    }

    public CSParameter(string name, ICSType csType, CSDefaultValue defaultValue = default)
    {
        _name = name;
        _rRefType = new(csType);
        _defaultValue = defaultValue;
    }


    public string Name => _name;
    public ICSType? Type => _rRefType.Output;
    public CSDefaultValue DefaultValue => _defaultValue;


    public override void OnSecondPass(CSTranslationUnit unit)
    {
           
    }
}