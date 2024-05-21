using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinTypedefs;
using CapiGenerator.CModel.Type;
using CapiGenerator.UtilTypes;
using CppAst;

namespace CapiGenerator.Parser;

public sealed class CCompilationUnit :
    IResolver<ICType, string>,
    IResolver<BaseCConstant, string>,
    IResolver<ICConstAssignable, string>
{
    private class ParserOutputChannel(CCompilationUnit compilationUnit) : BaseParserOutputChannel
    {
        private readonly List<BaseCConstant> _receiveConstants = [];
        private readonly List<CEnum> _receiveEnums = [];
        private readonly List<CStruct> _receiveStructs = [];
        private readonly List<CFunction> _receiveFunctions = [];
        private readonly List<CTypedef> _receiveTypedefs = [];


        public override void OnReceiveConstant(ReadOnlySpan<BaseCConstant> constants)
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
                if (compilationUnit._types.ContainsKey(@enum.Name))
                {
                    continue;
                }

                if (compilationUnit._typedefs.ContainsKey(@enum.Name))
                {
                    continue;
                }

                compilationUnit._types.Add(@enum.Name, @enum);
                compilationUnit._enums.Add(@enum.Name, @enum);
                foreach (var field in @enum.Fields)
                {
                    compilationUnit._enumFields.Add(field.Name, field);
                }
                _receiveEnums.Add(@enum);
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

                if (compilationUnit._structs.ContainsKey(@struct.Name))
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
            foreach (var type in types)
            {
                if (compilationUnit._types.ContainsKey(type.Name))
                {
                    continue;
                }

                if (compilationUnit._typedefs.ContainsKey(type.Name))
                {
                    continue;
                }

                compilationUnit._types.Add(type.Name, type);
                compilationUnit._typedefs.Add(type.Name, type);
                _receiveTypedefs.Add(type);
            }
        }

        public override void OnReceiveBuiltinTypedef(ReadOnlySpan<BaseBuiltinTypedef> types)
        {
            foreach (var type in types)
            {
                if(compilationUnit._builtinTypedefs.TryAdd(type.Name, type))
                {
                    compilationUnit._types.Add(type.Name, type);
                }
            }
        }

        public BaseCConstant[] ReceiveConstantsToArray()
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

        public CFunction[] ReceiveFunctionsToArray()
        {
            return [.. _receiveFunctions];
        }

        public CTypedef[] ReceiveTypedefsToArray()
        {
            return [.. _receiveTypedefs];
        }
    }

    public CCompilationUnit()
    {
        foreach (var item in CPrimitiveType.GetAllTypes())
        {
            _types.Add(item.Name, item);
        }
    }

    private sealed class ParserInputChannel(
        BaseCConstant[]? constants,
        CEnum[]? enums,
        CStruct[]? structs,
        CFunction[]? functions,
        CTypedef[]? typedefs
    ) : BaseParserInputChannel
    {
        public override ReadOnlySpan<BaseCConstant> GetConstants() =>
            constants is null ? ReadOnlySpan<BaseCConstant>.Empty : constants;

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

    private readonly Dictionary<string, BaseCConstant> _constants = [];
    private readonly Dictionary<string, CEnum> _enums = [];
    private readonly Dictionary<string, CEnumField> _enumFields = [];
    private readonly Dictionary<string, CStruct> _structs = [];
    private readonly Dictionary<string, CFunction> _functions = [];
    private readonly Dictionary<string, CTypedef> _typedefs = [];
    private readonly Dictionary<string, BaseBuiltinTypedef> _builtinTypedefs = [];

    private readonly List<BaseParser> _parsers = [];

    public ICType? GetTypeByName(string name) =>
        _types.TryGetValue(name, out var type) ? type : null;

    public BaseCConstant? GetConstantByName(string name) =>
        _constants.TryGetValue(name, out var constant) ? constant : null;

    public CEnum? GetEnumByName(string name) =>
        _enums.TryGetValue(name, out var @enum) ? @enum : null;

    public CEnumField? GetEnumFieldByName(string name) =>
        _enumFields.TryGetValue(name, out var field) ? field : null;

    public CStruct? GetStructByName(string name) =>
        _structs.TryGetValue(name, out var @struct) ? @struct : null;

    public CFunction? GetFunctionByName(string name) =>
        _functions.TryGetValue(name, out var function) ? function : null;

    public CTypedef? GetTypedefByName(string name) =>
        _typedefs.TryGetValue(name, out var typedef) ? typedef : null;


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
            parser.FirstPass(compilations, outputChannel);

            ParserInputChannel inputChannel = new(
                constants: outputChannel.ReceiveConstantsToArray(),
                enums: outputChannel.ReceiveEnumsToArray(),
                structs: outputChannel.ReceiveStructsToArray(),
                functions: outputChannel.ReceiveFunctionsToArray(),
                typedefs: outputChannel.ReceiveTypedefsToArray()
            );
            parserChannels.Add((parser, inputChannel));
        }

        foreach (var (parser, inputChannel) in parserChannels)
        {
            parser.SecondPass(this, inputChannel);
        }
    }


    public IEnumerable<ICType> GetTypeEnumerable() => _types.Values;
    public IEnumerable<BaseCConstant> GetConstantEnumerable() => _constants.Values;
    public IEnumerable<CEnum> GetEnumEnumerable() => _enums.Values;
    public IEnumerable<CStruct> GetStructEnumerable() => _structs.Values;
    public IEnumerable<CFunction> GetFunctionEnumerable() => _functions.Values;
    public IEnumerable<CTypedef> GetTypedefEnumerable() => _typedefs.Values;

    ICType? IResolver<ICType, string>.Resolve([DisallowNull] string key)
    {
        return GetTypeByName(key);
    }

    BaseCConstant? IResolver<BaseCConstant, string>.Resolve(string key)
    {
        return GetConstantByName(key);
    }

    public ICConstAssignable? Resolve([DisallowNull] string key)
    {
        return GetConstantByName(key) ?? (ICConstAssignable?)GetEnumFieldByName(key);
    }
}