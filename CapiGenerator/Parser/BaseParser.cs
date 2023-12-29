using CppAst;

namespace CapiGenerator.Parser;

public abstract class BaseParser
{
    public abstract void Parse(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel);

}
