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
        IReadOnlyDictionary<string, CppMacro>? macroFunctions = null,
        IReadOnlySet<string>? typedefNames = null)
    {
        var cppTokens = macro.Tokens
            .Where(token => token.Kind != CppTokenKind.Comment)
            .ToArray();
        if (macroFunctions is not null &&
            !MacroFunctionExpander.TryExpand(cppTokens, macroFunctions, out cppTokens))
        {
            return null;
        }

        if (!BaseCConstantToken.TryConvert(
                cppTokens,
                typedefNames ?? new HashSet<string>(),
                macro.Span.Start,
                out var constantTokens))
        {
            return null;
        }

        var resolvedTokens = MacroFunctionResolver.ResolveMacroFunction(constantTokens);
        if (resolvedTokens.Length == 0)
        {
            return null;
        }

        return new(macro, macro.Name, new(resolvedTokens))
        {
            Comment = null // TODO: CppAst does not support macro comments we need to parse them manually
        };
    }

}
