using CppAst;

namespace CapiGenerator.Parser;

public abstract class BaseParser
{
    private ParserCollection? _parsers;

    protected ParserCollection Parsers =>
        _parsers ?? throw new InvalidOperationException("Parser has not been initialized");

    public virtual void Init(ParserCollection parsers)
    {
        _parsers = parsers;
    }

    public abstract void FirstPass(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel);

    public virtual void SecondPass(
        CCompilationUnit compilationUnit,
        BaseParserInputChannel inputChannel
    )
    { }
}
