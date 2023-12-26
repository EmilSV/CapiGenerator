using CapiGenerator.Model;

namespace CapiGenerator.ConstantToken;

public class ConstIdentifierToken : BaseConstantToken
{
    private Constant? _constantModel;
    private readonly string? _constIdentifierName;

    public ConstIdentifierToken(string constIdentifierName)
    {
        _constIdentifierName = constIdentifierName;
    }

    public ConstIdentifierToken(Constant constantIdentifier)
    {
        _constantModel = constantIdentifier;
    }

    public Constant? GetConstantModel()
    {
        return _constantModel;
    }

    public override string? GetOutValue()
    {
        return _constantModel?.GetConstantIdentifierValue();
    }

    public override void OnSecondPass(IReadOnlyDictionary<string, Constant> constants)
    {
        if (_constIdentifierName == null)
        {
            return;
        }

        if (constants.TryGetValue(_constIdentifierName, out var value))
        {
            _constantModel = value;
        }
    }
}