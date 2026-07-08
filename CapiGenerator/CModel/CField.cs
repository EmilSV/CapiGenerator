using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;
using CppAst;
using CapiGenerator.CModel.Comments;

namespace CapiGenerator.CModel;

public class CField(string name, CTypeInstance type)
    : BaseCAstItem
{
    public readonly string Name = name;
    private readonly CTypeInstance _type = type;
    public CBaseComment? Comments { get; init; }

    public CTypeInstance GetFieldType()
    {
        return _type;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        _type.OnSecondPass(compilationUnit);
    }

    public static CField FromCppField(
        CppClass parent,
        CppField field,
        NestedCTypeFactory nestedTypeFactory,
        List<ICType> nestedTypes)
    {
        var (fieldType, modifiers) = field.Type.UnpackModifiers();
        if (nestedTypeFactory.TryCreateNestedTypeForFieldType(parent, field, fieldType, out var nestedType))
        {
            if (!nestedTypes.Contains(nestedType))
            {
                nestedTypes.Add(nestedType);
            }

            return new CField(field.Name, new CTypeInstance(nestedType, modifiers))
            {
                Comments = CBaseComment.From(field.Comment)
            };
        }

        return new CField(field.Name, CTypeInstance.FromCppType(field.Type))
        {
            Comments = CBaseComment.From(field.Comment)
        };
    }
}
