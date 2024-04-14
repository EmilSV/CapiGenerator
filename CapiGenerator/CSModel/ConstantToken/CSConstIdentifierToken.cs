using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.ConstantToken;

public class CSConstIdentifierToken : BaseCSConstantToken
{
    private readonly ResoleRef<ICSFieldLike, ICConstAssignable> _constantField;
    private CSFullFieldPath? _fullFieldPath;

    public CSConstIdentifierToken(CConstant cConstIdentifier)
    {
        _constantField = new(cConstIdentifier);
    }

    public CSConstIdentifierToken(CSFullFieldPath fullFieldPath)
    {
        _fullFieldPath = fullFieldPath;
    }

    public ICSFieldLike? GetField()
    {
        return _constantField.Output;
    }

    public override void OnSecondPass(CSTranslationUnit translationUnit)
    {
        if (_constantField.IsNull || _fullFieldPath != null)
        {
            return;
        }

        _constantField.TrySetOutputFromResolver(translationUnit);
        if (_constantField.TryAsBaseResolveRef(out var resolveRef) && resolveRef.IsOutputResolved())
        {
            var key = resolveRef.Key;
            CSBaseType? parent = null;
            if (key is BaseCAstItem cAstItem)
            {
                parent = cAstItem.EnrichingDataStore.Get<CSTranslationParentClassData>()?.Parent;
            }

            if (parent is not null)
            {
                var output = resolveRef.Output!;
                _fullFieldPath = new CSFullFieldPath(parent, output);
            }
        }
    }

    public static CSConstIdentifierToken FromCConstantToken(CConstIdentifierToken token)
    {
        return new CSConstIdentifierToken(token.GetConstantModel() ?? throw new InvalidOperationException("Constant model is not resolved"));
    }

    public override string? ToString()
    {
        return _fullFieldPath?.GetFullName();
    }
}