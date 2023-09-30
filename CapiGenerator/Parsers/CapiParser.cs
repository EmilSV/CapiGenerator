using CapiGenerator.Model;

namespace CapiGenerator.Parsers;

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
        var result = new CapiParserResult();
        var lookup = new GuidRef<Constant>.LookupCollection();
        var constants = _constantParser.Parse(args);
        result.Constants.AddRange(constants);
        return result;
    }
}