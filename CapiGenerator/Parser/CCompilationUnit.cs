using System.Reflection.Metadata;
using CapiGenerator.Model;
using CppAst;

namespace CapiGenerator.Parser;

public sealed class CCompilationUnit
{
    private class ParserOutputChannel(CCompilationUnit compilationUnit) : BaseParserOutputChannel
    {
        private readonly List<CConstant> _receiveConstants = [];
        private readonly List<CEnum> _receiveEnums = [];
        private readonly List<CStruct> _receiveStructs = [];
        private readonly List<ICType> _receiveTypes = [];
        private readonly List<CFunction> _receiveFunctions = [];


        public override void OnReceiveConstant(ReadOnlySpan<CConstant> constants)
        {
            foreach (var constant in constants)
            {
                if (compilationUnit._constants.TryAdd(constant.Name, constant))
                {
                    _receiveConstants.Add(constant);
                }
            }
        }

        public override void OnReceiveEnum(ReadOnlySpan<CEnum> enums)
        {
            foreach (var @enum in enums)
            {
                if (compilationUnit._enums.TryAdd(@enum.Name, @enum))
                {
                    _receiveEnums.Add(@enum);
                }
            }

        }

        public override void OnReceiveFunction(ReadOnlySpan<CFunction> functions)
        {
            foreach (var type in functions)
            {
                if (compilationUnit._functions.TryAdd(type.Name, type))
                {
                    _receiveFunctions.Add(type);
                }
            }
        }

        public override void OnReceiveStruct(ReadOnlySpan<CStruct> structs)
        {
            foreach (var @struct in structs)
            {
                if (compilationUnit._types.ContainsKey(@struct.Name))
                {
                    continue;
                }

                if(compilationUnit._structs.ContainsKey(@struct.Name))
                {
                    continue;
                }

                compilationUnit._types.Add(@struct.Name, @struct);
                compilationUnit._structs.Add(@struct.Name, @struct);
                _receiveStructs.Add(@struct);
            }
        }

        public override void OnReceiveTypedef(ReadOnlySpan<CTypedef> types)
        {
            throw new NotImplementedException();
        }

        public CConstant[] ReceiveConstantsToArray()
        {
            return [.. _receiveConstants];
        }

        public CEnum[] ReceiveEnumsToArray()
        {
            return [.. _receiveEnums];
        }

        public CStruct[] ReceiveStructsToArray()
        {
            return [.. _receiveStructs];
        }

        
    }

    private sealed class ParserInputChannel(
        CConstant[]? constants,
        CEnum[]? enums,
        CStruct[]? structs,
        CFunction[]? functions,
        CTypedef[]? typedefs
    ) : BaseParserInputChannel
    {
        public override ReadOnlySpan<CConstant> GetConstants() =>
            constants is null ? ReadOnlySpan<CConstant>.Empty : constants;

        public override ReadOnlySpan<CEnum> GetEnums() =>
            enums is null ? ReadOnlySpan<CEnum>.Empty : enums;

        public override ReadOnlySpan<CFunction> GetFunctions() =>
            functions is null ? ReadOnlySpan<CFunction>.Empty : functions;

        public override ReadOnlySpan<CStruct> GetStructs() =>
            structs is null ? ReadOnlySpan<CStruct>.Empty : structs;

        public override ReadOnlySpan<CTypedef> GetTypedefs() =>
            typedefs is null ? ReadOnlySpan<CTypedef>.Empty : typedefs;
    }

    public readonly Guid CompilationUnitId = Guid.NewGuid();
    private readonly Dictionary<string, ICType> _types = [];

    private readonly Dictionary<string, CConstant> _constants = [];
    private readonly Dictionary<string, CEnum> _enums = [];
    private readonly Dictionary<string, CStruct> _structs = [];
    private readonly Dictionary<string, CFunction> _functions = [];

    private readonly List<BaseParser> _parsers = [];

    public ICType? GetTypeByName(string name) =>
        _types.TryGetValue(name, out var type) ? type : null;

    public CConstant? GetConstantByName(string name) =>
        _constants.TryGetValue(name, out var constant) ? constant : null;

    public CEnum? GetEnumByName(string name) =>
        _enums.TryGetValue(name, out var @enum) ? @enum : null;

    public CStruct? GetStructByName(string name) =>
        _structs.TryGetValue(name, out var @struct) ? @struct : null;


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
            ParserOutputChannel outputChannel = new(this);
            parser.FirstPass(CompilationUnitId, compilations, outputChannel);

            ParserInputChannel inputChannel = new(
                constants: outputChannel.ReceiveConstantsToArray(),
                enums: outputChannel.ReceiveEnumsToArray(),
                structs: outputChannel.ReceiveStructsToArray(),
                types: outputChannel.ReceiveTypesToArray()
            );
            parserChannels.Add((parser, inputChannel));
        }

        foreach (var (parser, inputChannel) in parserChannels)
        {
            parser.SecondPass(this, inputChannel);
        }
    }
}