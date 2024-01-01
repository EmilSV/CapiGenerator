using System.Reflection.Metadata;
using CapiGenerator.Model;
using CppAst;

namespace CapiGenerator.Parser;

public sealed class CCompilationUnit
{
    private class ParserOutputChannel(CCompilationUnit compilationUnit) : BaseParserOutputChannel
    {
        private readonly CCompilationUnit _compilationUnit = compilationUnit;
        private readonly List<CConstant> _receiveConstants = [];

        public override void OnReceiveConstant(ReadOnlySpan<CConstant> constants)
        {
            foreach (var constant in constants)
            {
                _compilationUnit._constants.Add(constant.Name, constant);
            }
        }

        public CConstant[] ReceiveConstantsToArray()
        {
            return [.. _receiveConstants];
        }
    }

    private sealed class ParserInputChannel(CConstant[] constants) : BaseParserInputChannel
    {
        public override ReadOnlySpan<CConstant> GetConstants()
        {
            return constants is null ? ReadOnlySpan<CConstant>.Empty : constants;
        }
    }

    private readonly Dictionary<string, CConstant> _constants = [];


    private readonly List<BaseParser> _parsers = [];

    public ICFieldType GetFieldType(string name) =>
        throw new NotImplementedException("This method is not implemented.");

    public ICParameterType GetParameterType(string name) =>
        throw new NotImplementedException("This method is not implemented.");

    public CConstant GetConstant(string name) =>
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

    public void Parse(ReadOnlySpan<CppCompilation> compilations)
    {
        List<(BaseParser, ParserInputChannel)> parserChannels = [];

        foreach (var parser in _parsers)
        {
            var outputChannel = new ParserOutputChannel(this);
            parser.Parse(compilations, outputChannel);

            var inputChannel = new ParserInputChannel(outputChannel.ReceiveConstantsToArray());
            parserChannels.Add((parser, inputChannel));
        }

        foreach (var (parser, inputChannel) in parserChannels)
        {
            parser.OnSecondPass(this, inputChannel);
        }
    }
}