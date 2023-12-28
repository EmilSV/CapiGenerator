using CapiGenerator.Model;
using CapiGenerator.Model.ConstantToken;
using CppAst;

namespace CapiGenerator.Parser;


public class ConstantParser
{
    public CConst[] Parse(ReadOnlySpan<CppCompilation> compilations)
    {
        var constants = new List<CConst>();

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
                    constants.Add(newConst);
                }
            } 
        }

        return constants.ToArray();
    }

    public void OnSecondPass(CCompilationUnit compilationUnit, ReadOnlySpan<CConst> constants)
    {
        foreach (var constant in constants)
        {
            SecondPass(constant, compilationUnit);
        }
    }


    protected virtual void SecondPass(CConst value, CCompilationUnit compilationUnit)
    {
        value.OnSecondPass(compilationUnit);
    }

    protected virtual CConst? FirstPass(CppMacro macro)
    {
        var constantTokens = macro.Tokens.Select(CppTokenToConstantToken).ToArray();
        if (constantTokens == null || constantTokens.Any(token => token is null))
        {
            OnError(macro, "Failed to parse tokens");
            return null;
        }

        return new CConst(macro.Name, constantTokens!);
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