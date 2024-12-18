using CppAst;

namespace CapiGenerator.Parser;

public abstract class BaseParser
{
    public abstract void FirstPass(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel);

    public virtual void SecondPass(
        CCompilationUnit compilationUnit,
        BaseParserInputChannel inputChannel
    )
    { }
}
