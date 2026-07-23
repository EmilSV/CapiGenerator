using CapiGenerator.Parser;
using CppAst;

namespace CapiGenerator.CModel.ConstantToken;

public abstract class BaseCConstantToken(CppSourceLocation sourceLocation)
{
    public CppSourceLocation SourceLocation { get; } = sourceLocation;

    public virtual void OnSecondPass(CCompilationUnit compilationUnit)
    {

    }

    public static BaseCConstantToken? From(CppToken token, CppSourceLocation debugInfo) => token.Kind switch
    {
        CppTokenKind.Identifier => new CConstIdentifierToken(token.Text, debugInfo),
        CppTokenKind.Literal => new CConstLiteralToken(token.Text, debugInfo),
        CppTokenKind.Punctuation when
            CConstantPunctuationToken.TryParse(token.Text, debugInfo, out var punctuationToken) => punctuationToken,
        _ => null
    };
}
