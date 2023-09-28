using System.Collections.ObjectModel;
using System.Diagnostics;
using CapiGenerator.Model;
using CppAst;

namespace CapiGenerator.Parsers;

public class ConstantParser
{
    private readonly static HashSet<string> ValidIntPunctuation = new()
    {
        "+",
        "-",
        "*",
        "/",
        "%",
        "&",
        "|",
        "^",
        "~",
        "<<",
        ">>",
        "(",
        ")",
    };


    public Constant[] Create(CppCompilation compilation)
    {
        List<Constant> constants = new();

        List<CppMacro> potentialConstants = new();
        Dictionary<string, CppMacro> nameLookup = new();

        foreach (var constant in compilation.Macros)
        {
            if (constant.Parameters != null && constant.Parameters.Count > 0)
            {
                continue;
            }

            if (ShouldSkip(constant))
            {
                continue;
            }

            if (constant.Name.StartsWith("COMPLEX_EXPRESION_REF"))
            {
                Debugger.Break();
            }

            nameLookup.Add(constant.Name, constant);
            potentialConstants.Add(constant);
        }

        foreach (var constant in compilation.Macros)
        {
            TryParseMarcoValue(constant, nameLookup, out var type, out var parsedValue);
        }


        return Array.Empty<Constant>();
    }

    protected virtual bool ShouldSkip(CppMacro constant)
    {
        return false;
    }

    protected virtual bool TryParseMarcoValue(
        CppMacro value,
        IReadOnlyDictionary<string, CppMacro> otherMacros,
         out Type? type, out string? parsedValue)
    {

    }

    private static bool IsIntType(
        CppMacro value,
        IReadOnlyDictionary<string, CppMacro> otherMacros,
        HashSet<string> visitedMacros,
        Dictionary<string, bool> isIntCache)
    {
        if (isIntCache.TryGetValue(value.Name, out var output))
        {
            return output;
        }

        if (visitedMacros.Contains(value.Name))
        {
            return false;
        }

        visitedMacros.Add(value.Name);

        foreach (var token in value.Tokens)
        {
            switch (token.Kind)
            {
                case CppTokenKind.Comment:
                    break;
                case CppTokenKind.Identifier:

                    if (!otherMacros.TryGetValue(token.Text, out var macro))
                    {
                        isIntCache.Add(value.Name, false);
                        return false;
                    }

                    if (!IsIntType(macro, otherMacros, visitedMacros, isIntCache))
                    {
                        isIntCache.Add(value.Name, false);
                        return false;
                    }
                    break;
                case CppTokenKind.Keyword:
                case CppTokenKind.Literal:
                    return false;

                case CppTokenKind.Punctuation:
                    if (!ValidIntPunctuation.Contains(token.Text))
                    {
                        isIntCache.Add(value.Name, false);
                        return false;
                    }
                    break;

                default:
                    isIntCache.Add(value.Name, false);
                    return false;
            }
        }
        isIntCache.Add(value.Name, false);
        return true;
    }
}
