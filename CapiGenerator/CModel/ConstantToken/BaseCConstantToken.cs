using CapiGenerator.Parser;
using CppAst;

namespace CapiGenerator.CModel.ConstantToken;

public abstract class BaseCConstantToken
{
    public virtual void OnSecondPass(CCompilationUnit compilationUnit)
    {

    }

    public static BaseCConstantToken? From(CppToken token) => token.Kind switch
    {
        CppTokenKind.Identifier => new CConstIdentifierToken(token.Text),
        CppTokenKind.Literal => new CConstLiteralToken(token.Text),
        CppTokenKind.Punctuation when
            CConstantPunctuationToken.TryParse(token.Text, out var punctuationToken) => punctuationToken,
        _ => null
    };
}
