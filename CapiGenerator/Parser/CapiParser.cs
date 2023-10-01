using CapiGenerator.Model;

namespace CapiGenerator.Parser;

public class CapiParser
{
    private ConstantParser _constantParser = new();
    public CapiParser SetConstantParser(ConstantParser constantParser)
    {
        _constantParser = constantParser;
        return this;
    }

    internal CapiParserResult Parse(ParseArgs args)
    {
        var constants = _constantParser.Parse(args);
        var result = new CapiParserResult()
        {
            ConstLookup = constants
        };
        return result;
    }
}