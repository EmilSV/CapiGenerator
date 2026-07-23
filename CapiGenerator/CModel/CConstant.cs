using CapiGenerator.CModel.Comments;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Parser;
using CppAst;

namespace CapiGenerator.CModel;

public class CConstant(object primarySource, string name, CConstantExpression expression)
    : BaseCConstant(primarySource)
{
    public override string Name => name;

    public CConstantExpression Expression => expression;

    public CBaseComment? Comment { get; init; }

    public override CConstantType GetCConstantType()
    {
        return expression.GetTypeOfExpression();
    }

    public override void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var token in expression.Tokens)
        {
            token.OnSecondPass(compilationUnit);
        }
    }

    public static CConstant? From(
        CppMacro macro,
        IReadOnlyDictionary<string, CppMacro>? macroFunctions = null)
    {
        var cppTokens = macro.Tokens
            .Where(token => token.Kind != CppTokenKind.Comment)
            .ToArray();
        if (macroFunctions is not null &&
            !MacroFunctionExpander.TryExpand(cppTokens, macroFunctions, out cppTokens))
        {
            return null;
        }

        var constantTokens = cppTokens
            .Select(token => BaseCConstantToken.From(token, macro.Span.Start))
            .ToArray();
        if (constantTokens == null || constantTokens.Any(token => token is null))
        {
            return null;
        }

        var resolvedTokens = MacroFunctionResolver.ResolveMacroFunction(
            constantTokens.Select(token => token!).ToArray()
        );
        if (resolvedTokens.Length == 0)
        {
            return null;
        }

        MarkCastCandidates(resolvedTokens);

        return new(macro, macro.Name, new(resolvedTokens))
        {
            Comment = null // TODO: CppAst does not support macro comments we need to parse them manually
        };
    }

    private static void MarkCastCandidates(ReadOnlySpan<BaseCConstantToken> tokens)
    {
        for (var i = 1; i < tokens.Length - 1; i++)
        {
            if (tokens[i - 1] is CConstantPunctuationToken { Type: CPunctuationType.LeftParenthesis } &&
                tokens[i] is CConstIdentifierToken identifier &&
                tokens[i + 1] is CConstantPunctuationToken { Type: CPunctuationType.RightParenthesis })
            {
                identifier.MarkAsCastCandidate();
            }
        }
    }
}
