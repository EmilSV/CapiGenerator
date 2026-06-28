using System.Diagnostics.CodeAnalysis;
using CapiGenerator.Parser;
using CppAst;
namespace CapiGenerator.CModel;

public sealed class AnonymousCTypeFactory
{
    private readonly ParserCollection _parserCollection;

    public AnonymousCTypeFactory(ParserCollection parserCollection)
    {
        _parserCollection = parserCollection;
    }

    public bool TryCreateAnonymousTypeForFieldType(CppField field, [NotNullWhen(true)] out ICType? anonymousType)
    {

        var type = field.Type;
        if (type is not CppClass { IsAnonymous: true } cppClass)
        {
            anonymousType = null;
            return false;
        }

        var typeName = CreateTypeNameFromFieldName(field.Name, "AnonymousStruct");
        anonymousType = cppClass.ClassKind switch
        {
            CppClassKind.Struct => _parserCollection.GetParser<StructParser>()?.CppClassToCStruct(cppClass, typeName, true),
            CppClassKind.Union => _parserCollection.GetParser<UnionParser>()?.CppClassToCUnion(cppClass, typeName, true),
            _ => null,
        };
        return anonymousType is not null;
    }

    private static string CreateTypeNameFromFieldName(string? fieldName, string fallbackName) => fieldName switch
    {
        null or "" => fallbackName,
        _ when char.IsDigit(fieldName[0]) => $"_{fieldName}",
        _ => char.ToUpper(fieldName[0]) + fieldName[1..],
    };
}
