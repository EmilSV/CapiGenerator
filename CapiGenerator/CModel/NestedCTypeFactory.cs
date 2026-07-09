using System.Diagnostics.CodeAnalysis;
using CapiGenerator.Parser;
using CppAst;

namespace CapiGenerator.CModel;

public sealed class NestedCTypeFactory
{
    private readonly ParserCollection _parserCollection;
    private readonly Dictionary<CppClass, ICType> _nestedTypesByClass = [];

    public NestedCTypeFactory(ParserCollection parserCollection)
    {
        _parserCollection = parserCollection;
    }

    public bool TryCreateNestedTypeForFieldType(
        CppClass parent,
        CppField field,
        CppType fieldType,
        [NotNullWhen(true)] out ICType? nestedType)
    {
        if (fieldType is not CppClass cppClass || !ShouldTreatAsNested(parent, cppClass))
        {
            nestedType = null;
            return false;
        }

        if (_nestedTypesByClass.TryGetValue(cppClass, out nestedType))
        {
            return true;
        }

        var typeName = cppClass.IsAnonymous
            ? CreateTypeNameFromFieldName(field.Name, "AnonymousStruct")
            : cppClass.Name;

        nestedType = cppClass.ClassKind switch
        {
            CppClassKind.Struct => _parserCollection.GetParser<StructParser>()?.CppClassToCStruct(cppClass, typeName, cppClass.IsAnonymous, field),
            CppClassKind.Union => _parserCollection.GetParser<UnionParser>()?.CppClassToCUnion(cppClass, typeName, cppClass.IsAnonymous, field),
            _ => null,
        };

        if (nestedType is null)
        {
            return false;
        }

        _nestedTypesByClass.Add(cppClass, nestedType);
        return true;
    }

    private static bool ShouldTreatAsNested(CppClass parent, CppClass cppClass)
    {
        if (cppClass.IsAnonymous)
        {
            return true;
        }

        return ReferenceEquals(cppClass.Parent, parent) || parent.Classes.Contains(cppClass);
    }

    private static string CreateTypeNameFromFieldName(string? fieldName, string fallbackName) => fieldName switch
    {
        null or "" => fallbackName,
        _ when char.IsDigit(fieldName[0]) => $"_{fieldName}",
        _ => char.ToUpper(fieldName[0]) + fieldName[1..],
    };
}
