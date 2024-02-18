using CapiGenerator.CModel;

namespace CapiGenerator.CSModel;

public class CSMethod : BaseCSAstItem
{
    private readonly ResoleRef<ICSType, ICType> _rRefReturnType;
    private readonly string _name;
    private readonly CSParameter[] _parameters;

    public CSMethod(ICType returnType, string name, ReadOnlySpan<CSParameter> parameters)
    {
        _rRefReturnType = new(returnType);
        _name = name;
        _parameters = parameters.ToArray();
    }

    public CSMethod(ICSType returnType, string name, ReadOnlySpan<CSParameter> parameters)
    {
        _rRefReturnType = new(returnType);
        _name = name;
        _parameters = parameters.ToArray();
    }

    public string Name => _name;
    public ResoleRef<ICSType, ICType> RRefReturnType => _rRefReturnType;
    public ICSType? ReturnType => _rRefReturnType.Output;
    public ReadOnlySpan<CSParameter> Parameters => _parameters;
}