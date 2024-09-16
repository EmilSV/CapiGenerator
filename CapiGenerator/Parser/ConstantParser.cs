using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.CModel.ConstantToken;
using CppAst;

namespace CapiGenerator.Parser;


public class ConstantParser : BaseParser
{
    public override void FirstPass(
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

                if (macro.Tokens.Count == 0)
                {
                    continue;
                }

                if (ShouldSkip(macro))
                {
                    continue;
                }

                var builtinConstant = AllBuiltinCConstants.AllCConstantTypes.FirstOrDefault(bc => bc.MacroIsBuiltin(macro));
                if (builtinConstant is not null)
                {
                    outputChannel.OnReceiveConstant(builtinConstant);
                    continue;
                }

                var newConst = FirstPass(macro);
                if (newConst is not null)
                {
                    outputChannel.OnReceiveConstant(newConst);
                }
            }

            foreach (var field in compilation.Fields)
            {
                if (field.StorageQualifier != CppStorageQualifier.Static)
                {
                    continue;
                }

                if (field.Type is not CppQualifiedType type || type.Qualifier != CppTypeQualifier.Const)
                {
                    continue;
                }

                if (ShouldSkip(field))
                {
                    continue;
                }

                

            }
        }
    }

    public override void SecondPass(CCompilationUnit compilationUnit, BaseParserInputChannel inputChannel)
    {
        foreach (var constant in inputChannel.GetConstants())
        {
            SecondPass(constant, compilationUnit);
        }
    }

    protected virtual void SecondPass(BaseCConstant value, CCompilationUnit compilationUnit)
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

        return new(macro.Name, new(constantTokens!));
    }

    protected virtual bool ShouldSkip(CppMacro constant) => false;
    protected virtual bool ShouldSkip(CppField constant) => false;
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