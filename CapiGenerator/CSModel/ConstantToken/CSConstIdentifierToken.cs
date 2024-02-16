using CapiGenerator.CModel;
using CapiGenerator.Parser;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel.ConstantToken;

public class CConstIdentifierToken : BaseCSConstantToken
{
    private ResoleRef<CSField, BaseCAstItem>? _constantField;
    private readonly string? _constIdentifierName;

    public CConstIdentifierToken(string constIdentifierName)
    {
        _constIdentifierName = constIdentifierName;
    }

    public CConstIdentifierToken(CSField constantField)
    {
        _constantField = constantField;
    }

    public CSField? GetField()
    {
        return _constantField;
    }

    public override void OnSecondPass(CSTranslationUnit translationUnit)
    {
        if (_constIdentifierName != null)
        {
            _constantModel = translationUnit.GetConstantByName(_constIdentifierName);
        }
    }
}