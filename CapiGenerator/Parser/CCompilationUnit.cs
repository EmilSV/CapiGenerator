using System.Reflection.Metadata;
using CapiGenerator.Model;

namespace CapiGenerator.Parser;

public sealed class CCompilationUnit
{
    private sealed class ParserInputChannel(CConst[] constants) : BaseParserInputChannel
    {
        private CConst[]? constants = constants;

        public override ReadOnlySpan<CConst> GetConstants()
        {
            return constants is null ? ReadOnlySpan<CConst>.Empty : constants;
        }
    }

    private readonly List<BaseParser> _parsers = [];

    public ICFieldType GetFieldType(string name) =>
        throw new NotImplementedException("This method is not implemented.");

    public ICParameterType GetParameterType(string name) =>
        throw new NotImplementedException("This method is not implemented.");

    public CConst GetConstant(string name) =>
        throw new NotImplementedException("This method is not implemented.");


    public CCompilationUnit AddParser(BaseParser parser)
    {
        _parsers.Add(parser);
        return this;
    }

    public CCompilationUnit AddParser(ReadOnlySpan<BaseParser> parsers)
    {
        foreach (var parser in parsers)
        {
            _parsers.Add(parser);
        }
        return this;
    }
}