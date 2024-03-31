using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.ConstantToken;

public class CSConstIdentifierToken : BaseCSConstantToken
{
    private readonly ResoleRef<ICSField, ICConstAssignable> _constantField;

    public CSConstIdentifierToken(CConstant cConstIdentifier)
    {
        _constantField = new(cConstIdentifier);
    }

    public CSConstIdentifierToken(CSField constantField)
    {
        _constantField = new(constantField);
    }

    public ICSField? GetField()
    {
        return _constantField.Output;
    }

    public override void OnSecondPass(CSTranslationUnit translationUnit)
    {
        _constantField.TrySetOutputFromResolver(translationUnit);
    }

    public static CSConstIdentifierToken FromCConstantToken(CConstIdentifierToken token)
    {
        return new CSConstIdentifierToken(token.GetConstantModel() ?? throw new InvalidOperationException("Constant model is not resolved"));
    }

    public override string? ToString()
    {
        return _constantField?.Output?.FullName;
    }
}