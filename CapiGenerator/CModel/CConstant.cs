using CapiGenerator.CModel.Comments;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.Parser;
using CppAst;

namespace CapiGenerator.CModel;

public class CConstant(string name, CConstantExpression expression)
    : BaseCConstant
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

    public static CConstant? From(CppMacro macro)
    {
        var constantTokens = macro.Tokens.Select(BaseCConstantToken.From).ToArray();
        if (constantTokens == null || constantTokens.Any(token => token is null))
        {
            return null;
        }

        constantTokens = MacroFunctionResolver.ResolveMacroFunction(constantTokens!);

        return new(macro.Name, new(constantTokens!))
        {
            Comment = null // TODO: CppAst does not support macro comments we need to parse them manually
        };
    }
}
