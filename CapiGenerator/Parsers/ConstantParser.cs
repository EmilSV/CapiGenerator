using System.Collections.ObjectModel;
using System.Diagnostics;
using CapiGenerator.Model;
using CppAst;

namespace CapiGenerator.Parsers;

public class ConstantParser
{
    public enum Type
    {
        NONE = 0,
        Int,
        Float,
        String,
    }

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

    private readonly static HashSet<string> ValidFloatPunctuation = new()
    {
        "+",
        "-",
        "*",
        "/",
        "%",
        "(",
        ")",
    };

    private readonly static HashSet<string> ValidStringPunctuation = new()
    {
        "+",
        "(",
        ")",
    };

    private readonly Dictionary<string, (Type, CppMacro?)> parsedCache = new();
    private readonly HashSet<string> visitedMacros = new();


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

            nameLookup.Add(constant.Name, constant);
            potentialConstants.Add(constant);
        }

        parsedCache.Clear();
        visitedMacros.Clear();

        foreach (var constant in compilation.Macros)
        {
            TryParseMarcoValue(constant, nameLookup, out var _);
        }


        return Array.Empty<Constant>();
    }

    protected virtual bool ShouldSkip(CppMacro constant) => false;

    protected virtual void OnError(CppMacro macro, string message) { }

    protected bool TryParseMarcoValue(
        CppMacro value,
        IReadOnlyDictionary<string, CppMacro> otherMacros,
        out Type type)
    {
        if (parsedCache.TryGetValue(value.Name, out var cached))
        {
            type = cached.Item1;
            return type != Type.NONE;
        }

        if (visitedMacros.Contains(value.Name))
        {
            type = default;
            return false;
        }

        visitedMacros.Add(value.Name);

        if (TryParseMarcoValueImpl(value, otherMacros, out type))
        {
            parsedCache.Add(value.Name, (type, value));
            return true;
        }
        else
        {
            parsedCache.Add(value.Name, (Type.NONE, null));
        }

        type = default;
        return false;
    }

    protected virtual bool TryParseMarcoValueImpl(
        CppMacro value,
        IReadOnlyDictionary<string, CppMacro> otherMacros,
         out Type type)
    {
        if (IsIntType(value, otherMacros))
        {
            type = Type.Int;
            return true;
        }
        else if (IsFloatType(value, otherMacros))
        {
            type = Type.Float;
            return true;
        }
        else if (IsStringType(value, otherMacros))
        {
            type = Type.String;
            return true;
        }
        else
        {
            type = default;
            return false;
        }
    }


    private bool IsIntType(
        CppMacro value,
        IReadOnlyDictionary<string, CppMacro> otherMacros)
    {
        foreach (var token in value.Tokens)
        {
            switch (token.Kind)
            {
                case CppTokenKind.Comment:
                    break;
                case CppTokenKind.Identifier:
                    if (!TryParseMarcoValue(value, otherMacros, out var type) && type != Type.Int)
                    {
                        return false;
                    }
                    break;
                case CppTokenKind.Keyword:
                case CppTokenKind.Literal:
                    if (token.Text.StartsWith('"') || token.Text.EndsWith('"'))
                    {
                        return false;
                    }
                    break;

                case CppTokenKind.Punctuation:
                    if (!ValidIntPunctuation.Contains(token.Text))
                    {
                        return false;
                    }
                    break;

                default:
                    return false;
            }
        }
        return true;
    }

    private bool IsFloatType(
        CppMacro value,
        IReadOnlyDictionary<string, CppMacro> otherMacros)
    {
        foreach (var token in value.Tokens)
        {
            switch (token.Kind)
            {
                case CppTokenKind.Comment:
                    break;
                case CppTokenKind.Identifier:

                    if (!TryParseMarcoValue(value, otherMacros, out var type) &&
                        type is not Type.Float or Type.Int)
                    {
                        return false;
                    }
                    break;
                case CppTokenKind.Keyword:
                case CppTokenKind.Literal:
                    if (token.Text.StartsWith('"') || token.Text.EndsWith('"'))
                    {
                        return false;
                    }
                    break;

                case CppTokenKind.Punctuation:
                    if (!ValidFloatPunctuation.Contains(token.Text))
                    {
                        return false;
                    }
                    break;

                default:
                    return false;
            }
        }
        return true;
    }

    private bool IsStringType(
        CppMacro value,
        IReadOnlyDictionary<string, CppMacro> otherMacros)
    {
        foreach (var token in value.Tokens)
        {
            switch (token.Kind)
            {
                case CppTokenKind.Comment:
                    break;
                case CppTokenKind.Identifier:
                    if (!TryParseMarcoValue(value, otherMacros, out var type) && type != Type.String)
                    {
                        return false;
                    }
                    break;
                case CppTokenKind.Keyword:
                case CppTokenKind.Literal:
                    if (!token.Text.StartsWith('"') || !token.Text.EndsWith('"'))
                    {
                        return false;
                    }
                    break;

                case CppTokenKind.Punctuation:
                    if (!ValidStringPunctuation.Contains(token.Text))
                    {
                        return false;
                    }
                    break;

                default:
                    return false;
            }
        }
        return true;
    }
}
