using CapiGenerator.Parser;
using CapiGenerator.CModel.Type;
using CppAst;

namespace CapiGenerator.CModel;

public class CField(string name, CTypeInstance type)
    : BaseCAstItem
{
    public readonly string Name = name;
    private readonly CTypeInstance _type = type;

    public CTypeInstance GetFieldType()
    {
        return _type;
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        _type.OnSecondPass(compilationUnit);
    }

    public static CField FromCppField(CppField field, AnonymousCTypeFactory anonymousTypeFactory)
    {
        if (field.Type is not CppClass { IsAnonymous: true })
        {
            return new CField(field.Name, CTypeInstance.FromCppType(field.Type));
        }
        if (anonymousTypeFactory.TryCreateAnonymousTypeForFieldType(field, out var anonymousType))
        {
            return new CField(field.Name, new CTypeInstance(anonymousType!, []));
        }
        throw new InvalidOperationException($"Failed to create anonymous type for field {field.Name}");
    }
}
