namespace CapiGenerator.Parser;

public sealed class ParserCollection
{
    private readonly List<BaseParser> _parsers = [];

    internal ParserCollection(List<BaseParser> parsers)
    {
        _parsers = parsers;
    }

    public T? GetParser<T>() where T : BaseParser
    {
        return _parsers.OfType<T>().FirstOrDefault();
    }

    public IEnumerable<T> GetParsers<T>() where T : BaseParser
    {
        return _parsers.OfType<T>();
    }
}
