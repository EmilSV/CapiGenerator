using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.ConstantToken;

public class CSConstIdentifierToken : BaseCSConstantToken
{
    private readonly ResoleRef<ICSFieldLike, ICConstAssignable> _constantField;

    public CSConstIdentifierToken(CConstant cConstIdentifier)
    {
        _constantField = new(cConstIdentifier);
    }

    public CSConstIdentifierToken(ICSFieldLike field)
    {
        _constantField = new(field);
    }

    public ICSFieldLike? GetField()
    {
        return _constantField.Output;
    }

    public override void OnSecondPass(CSTranslationUnit translationUnit)
    {
        if (_constantField.IsNull || _constantField.IsOutputResolved())
        {
            return;
        }

        _constantField.TrySetOutputFromResolver(translationUnit);
    }

    public static CSConstIdentifierToken FromCConstantToken(CConstIdentifierToken token)
    {
        return new CSConstIdentifierToken(token.GetConstantModel() ?? throw new InvalidOperationException("Constant model is not resolved"));
    }

    public override string? ToString() => 
        _constantField.Output?.GetFullName();
}