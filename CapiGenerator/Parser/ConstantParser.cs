using CapiGenerator.Model;
using CapiGenerator.Model.ConstantToken;
using CppAst;

namespace CapiGenerator.Parser;


public class ConstantParser
{
    public void Parse(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel)
    {
        foreach (var compilation in compilations)
        {
            foreach (var macro in compilation.Macros)
            {
                if (macro.Parameters != null && macro.Parameters.Count > 0)
                {
                    continue;
                }

                if (ShouldSkip(macro))
                {
                    continue;
                }

                var newConst = FirstPass(macro);
                if (newConst is not null)
                {
                    outputChannel.OnReceiveConstant(newConst);
                }
            }
        }
    }

    public void OnSecondPass(CCompilationUnit compilationUnit, BaseParserInputChannel inputChannel)
    {
        foreach (var constant in inputChannel.GetConstants())
        {
            SecondPass(constant, compilationUnit);
        }
    }

    protected virtual void SecondPass(CConstant value, CCompilationUnit compilationUnit)
    {
        value.OnSecondPass(compilationUnit);
    }

    protected virtual CConstant? FirstPass(CppMacro macro)
    {
        var constantTokens = macro.Tokens.Select(CppTokenToConstantToken).ToArray();
        if (constantTokens == null || constantTokens.Any(token => token is null))
        {
            OnError(macro, "Failed to parse tokens");
            return null;
        }

        return new CConstant(macro.Name, false, constantTokens!);
    }

    protected virtual bool ShouldSkip(CppMacro constant) => false;
    protected virtual void OnError(CppMacro macro, string message)
    {
        Console.Error.WriteLine($"Error parsing constant {macro.Name}: {message}");
    }

    public static BaseCConstantToken? CppTokenToConstantToken(CppToken token) => token.Kind switch
    {
        CppTokenKind.Identifier => new CConstIdentifierToken(token.Text),
        CppTokenKind.Literal => new CConstLiteralToken(token.Text),
        CppTokenKind.Punctuation when
            CConstantPunctuationToken.TryParse(token.Text, out var punctuationToken) => punctuationToken,
        _ => null
    };
}