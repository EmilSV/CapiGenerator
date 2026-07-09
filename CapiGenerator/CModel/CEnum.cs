using CapiGenerator.CModel.Comments;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Parser;
using CppAst;

namespace CapiGenerator.CModel;


public sealed class CEnum(object primarySource, string name, ReadOnlySpan<CEnumField> fields) :
    BaseCAstItem(primarySource), ICType
{
    public string Name => name;
    private readonly CEnumField[] _fields = fields.ToArray();
    public ReadOnlySpan<CEnumField> Fields => _fields;

    public CBaseComment? Comment { get; init; }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var value in _fields)
        {
            value.OnSecondPass(compilationUnit);
        }
    }

    public static CEnum? From(CppEnum astEnum)
    {
        var enumConstants = astEnum.Items.Select(CEnumField.From).ToArray();
        if (enumConstants == null || enumConstants.Any(token => token is null))
        {
            return null;
        }

        return new CEnum(astEnum, astEnum.Name, enumConstants!)
        {
            Comment = astEnum.Comment is not null ? CBaseComment.From(astEnum.Comment) : null
        };
    }
}
