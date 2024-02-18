using CapiGenerator.CModel;
using CapiGenerator.Parser;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel.ConstantToken;

public class CConstIdentifierToken : BaseCSConstantToken
{
    private readonly ResoleRef<CSField, BaseCAstItem> _constantField;

    public CConstIdentifierToken(BaseCAstItem cConstIdentifier)
    {
        _constantField = new(cConstIdentifier);
    }

    public CConstIdentifierToken(CSField constantField)
    {
        _constantField = new(constantField);
    }

    public CSField? GetField()
    {
        return _constantField.Output;
    }

    public override void OnSecondPass(CSTranslationUnit translationUnit)
    {
        _constantField.TrySetOutputFromResolver(translationUnit);
    }
}