using System.Collections.Immutable;
using System.Diagnostics;
using CapiGenerator.ConstantToken;
using CapiGenerator.Model;
using CapiGenerator.ModelFactory;
using CapiGenerator.OutFile;
using CppAst;


namespace CapiGenerator.Parser;

public class ConstantParser
{
    private readonly IConstantTypeResolver _constantTypeResolver;

    public ConstantParser(IConstantTypeResolver? constantTypeResolver = null)
    {
        _constantTypeResolver = constantTypeResolver ?? new DefaultConstTypeResolver();
    }

    public ConstModelFactory Parse(ParseArgs args)
    {
        List<CppMacro> potentialConstants = new();
        ConstModelFactory factory = new ConstModelFactory();

        var compilation = args.Compilation;

        foreach (var constant in compilation.Macros)
        {
            if (constant.Parameters != null && constant.Parameters.Count > 0)
            {
                continue;
            }

            if (ShouldSkip(constant, args))
            {
                continue;
            }

            potentialConstants.Add(constant);
        }

        foreach (CppMacro item in potentialConstants)
        {
            ParseMarcoValue(factory, item, args);
        }

        factory.RemoveAll(constant => constant is null);
        factory.RemoveAll(constant => constant.Output.Tokens.Length == 0);
        factory.RemoveAll(constant =>
        {
            if (constant.Input.Name == "TJ_444")
            {
                Debugger.Break();
            }

            bool isRemoved = constant.ResolveOutputType(_constantTypeResolver) is ConstantType.Unknown or ConstantType.NONE;

            return isRemoved;
        });

        return factory;
    }

    protected virtual bool ShouldSkip(CppMacro constant, ParseArgs args) => false;

    protected virtual void OnError(CppMacro macro, string message, ParseArgs args)
    {
        Console.Error.WriteLine($"Error parsing constant {macro.Name}: {message}");
    }

    protected virtual void ParseMarcoValue(ConstModelFactory factory, CppMacro value, ParseArgs args)
    {
        var compileUnitNamespace = args.CompileUnitNamespace;
        var lookup = args.Lookups;
        var outputFolder = args.OutputFolder;

        Constant.ConstantInput input = new()
        {
            Name = value.Name,
            DllName = compileUnitNamespace,
            Macro = value,
        };

        var constantTokens = value.Tokens.Select(
            token => CppTokenToConstantToken(token, compileUnitNamespace, factory)
        ).ToArray();

        if (constantTokens.Any(token => token is null))
        {
            OnError(value, $"Failed to parse constant {value.Name}", args);
        }

        Constant.ConstantOutput output = new()
        {
            Name = value.Name,
            OutputFile = outputFolder.GetFile<StaticClassCSharpOutFile>(compileUnitNamespace + "Consts"),
            Tokens = constantTokens!.ToImmutableArray()!,
        };

        factory.CreateConstant(input, output);
    }

    public static BaseConstantToken? CppTokenToConstantToken(
        CppToken token,
        string compileUnitNamespace,
        ConstModelFactory factory
    ) => token.Kind switch
    {
        CppTokenKind.Identifier => new ConstIdentifierToken()
        {
            Value = factory.GetRef(token.Text, compileUnitNamespace)
        },
        CppTokenKind.Literal => new ConstantLiteralToken(token.Text),
        CppTokenKind.Punctuation when
            ConstantPunctuationToken.TryParse(token.Text, out var punctuationToken) => punctuationToken,
        _ => null
    };
}
