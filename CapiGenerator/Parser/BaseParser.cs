using CppAst;

namespace CapiGenerator.Parser;

public abstract class BaseParser
{
    public abstract void Parse(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel);

    public abstract void OnSecondPass(
        CCompilationUnit compilationUnit,
        BaseParserInputChannel inputChannel
    );
}
