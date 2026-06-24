using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using CapiGenerator.CSModel;

namespace CapiGenerator.XmlComments;

public static partial class XmlCommentFinder
{
    public static ICommendableItem? FindComments(IXmlCommentsTypeProvider translationResult, string location)
    {
        location = RemoveWhitespace(location);

        foreach (var csEnum in translationResult.GetCSEnumsEnumerable())
        {
            var result = FindCommentsInType(csEnum, location);
            if (result is not null)
            {
                return result;
            }
        }

        foreach (var csStruct in translationResult.GetCSStructsEnumerable())
        {
            var result = FindCommentsInType(csStruct, location);
            if (result is not null)
            {
                return result;
            }
        }

        foreach (var staticClass in translationResult.GetCSStaticClassesEnumerable())
        {
            var result = FindCommentsInType(staticClass, location);
            if (result is not null)
            {
                return result;
            }
        }

        return null;
    }

    private static ICommendableItem? FindCommentsInType(BaseCSType type, string location)
    {
        var typeFullName = RemoveWhitespace(type.GetFullName());
        if (typeFullName == location)
        {
            return type;
        }

        switch (type)
        {
            case CSEnum csEnum:
                foreach (var field in csEnum.Values)
                {
                    var fullNameField = RemoveWhitespace(field.GetFullName());
                    if (fullNameField == location)
                    {
                        return field;
                    }
                }
                break;

            case CSStruct csStruct:
                var structMemberResult = FindCommentsInMemberContainer(csStruct, location);
                if (structMemberResult is not null)
                {
                    return structMemberResult;
                }

                foreach (var constructor in csStruct.Constructors)
                {
                    var fullName = RemoveWhitespace(constructor.GetFullName());
                    var fullNameWithParameters = RemoveWhitespace(constructor.GetFullNameWithParameters());
                    if (fullName == location || fullNameWithParameters == location)
                    {
                        return constructor;
                    }
                }

                foreach (var nestedType in csStruct.NestedTypes)
                {
                    var nestedResult = FindCommentsInType(nestedType, location);
                    if (nestedResult is not null)
                    {
                        return nestedResult;
                    }
                }
                break;

            case CSStaticClass csStaticClass:
                return FindCommentsInMemberContainer(csStaticClass, location);
        }

        return null;
    }

    private static ICommendableItem? FindCommentsInMemberContainer(BaseCSMemberContainer memberContainer, string location)
    {
        foreach (var field in memberContainer.Fields)
        {
            var fullNameField = RemoveWhitespace(field.GetFullName());
            if (fullNameField == location)
            {
                return field;
            }
        }

        foreach (var method in memberContainer.Methods)
        {
            var fullName = RemoveWhitespace(method.GetFullName());
            var fullNameWithParameters = RemoveWhitespace(method.GetFullNameWithParameters());
            if (fullName == location || fullNameWithParameters == location)
            {
                return method;
            }
        }

        return null;
    }

    [return: NotNullIfNotNull(nameof(str))]
    private static string? RemoveWhitespace(string? str)
    {
        if (str == null)
        {
            return str;
        }

        var RegexWhitespace = WhiteSpaceRegex();
        return RegexWhitespace.Replace(str, "");
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhiteSpaceRegex();
}
