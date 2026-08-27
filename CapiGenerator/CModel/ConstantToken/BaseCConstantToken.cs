using CapiGenerator.CModel.Type;
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

    public static bool TryConvert(
        IReadOnlyList<CppToken> cppTokens,
        IReadOnlySet<string> typedefNames,
        CppSourceLocation sourceLocation,
        out BaseCConstantToken[] constantTokens)
    {
        List<BaseCConstantToken> output = [];
        for (int i = 0; i < cppTokens.Count; i++)
        {
            if (TryCreateCastToken(
                    cppTokens,
                    i,
                    typedefNames,
                    sourceLocation,
                    out var castToken,
                    out var endIndex))
            {
                output.Add(castToken);
                i = endIndex;
                continue;
            }

            if (From(cppTokens[i], sourceLocation) is not { } token)
            {
                constantTokens = [];
                return false;
            }

            output.Add(token);
        }

        constantTokens = [.. output];
        return true;
    }

    private static bool TryCreateCastToken(
        IReadOnlyList<CppToken> tokens,
        int startIndex,
        IReadOnlySet<string> typedefNames,
        CppSourceLocation sourceLocation,
        out CConstCastToken castToken,
        out int endIndex)
    {
        castToken = null!;
        endIndex = -1;
        if (tokens[startIndex].Text != "(")
        {
            return false;
        }

        int typeStartIndex = startIndex + 1;
        int rightParenthesisIndex = typeStartIndex;
        while (rightParenthesisIndex < tokens.Count && tokens[rightParenthesisIndex].Text != ")")
        {
            if (tokens[rightParenthesisIndex].Kind != CppTokenKind.Identifier)
            {
                return false;
            }

            rightParenthesisIndex++;
        }

        if (rightParenthesisIndex == typeStartIndex || rightParenthesisIndex >= tokens.Count)
        {
            return false;
        }

        var typeName = string.Join(
            " ",
            tokens.Skip(typeStartIndex).Take(rightParenthesisIndex - typeStartIndex).Select(token => token.Text));
        if (TryGetPrimitiveType(typeName, out var primitiveType))
        {
            castToken = new(primitiveType, sourceLocation);
        }
        else if (rightParenthesisIndex == typeStartIndex + 1 &&
                 (typedefNames.Contains(typeName) || typeName == "size_t"))
        {
            castToken = new(typeName, sourceLocation);
        }
        else
        {
            return false;
        }

        endIndex = rightParenthesisIndex;
        return true;
    }

    private static bool TryGetPrimitiveType(string typeName, out CPrimitiveType primitiveType)
    {
        foreach (var type in CPrimitiveType.GetAllTypes())
        {
            foreach (var name in type.Names)
            {
                if (name == typeName)
                {
                    primitiveType = type;
                    return true;
                }
            }
        }

        primitiveType = null!;
        return false;
    }
}
