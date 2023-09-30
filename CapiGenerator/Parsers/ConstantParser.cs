using System.Collections.Immutable;
using CapiGenerator.ConstantToken;
using CapiGenerator.Model;
using CppAst;

using ConstLookup = CapiGenerator.GuidRef<CapiGenerator.Model.Constant>.LookupCollection;

namespace CapiGenerator.Parsers;

public class ConstantParser
{
    public Constant[] Parse(ParseArgs args)
    {
        List<Constant> constants = new();
        List<CppMacro> potentialConstants = new();

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
            var constant = ParseMarcoValue(item, args);
            if (constant is not null)
            {
                constants.Add(constant);
            }
        }

        return constants.ToArray();
    }

    protected virtual bool ShouldSkip(CppMacro constant, ParseArgs args) => false;

    protected virtual void OnError(CppMacro macro, string message, ParseArgs args)
    {
        Console.Error.WriteLine($"Error parsing constant {macro.Name}: {message}");
    }

    protected virtual Constant? ParseMarcoValue(CppMacro value, ParseArgs args)
    {
        var compileUnitNamespace = args.CompileUnitNamespace;
        var lookup = args.Lookups;
        var outputFolder = args.OutputFolder;

        Constant.ConstantInput input = new()
        {
            Name = value.Name,
            CompileUnitNamespace = compileUnitNamespace,
            Macro = value,
        };

        var constantTokens = value.Tokens.Select(
            token => CppTokenToConstantToken(token, compileUnitNamespace, lookup.ConstLookup)
        ).ToArray();

        if (constantTokens.Any(token => token is null))
        {
            OnError(value, $"Failed to parse constant {value.Name}", args);
            return null;
        }

        Constant.ConstantOutput output = new()
        {
            Name = value.Name,
            OutputClassName = value.Name,
            OutputFile = outputFolder.GetFile(compileUnitNamespace + "Consts", ClassType.StaticClass),
            Tokens = constantTokens!.ToImmutableArray()!,
        };

        return new Constant(lookup.ConstLookup, input, output);
    }

    public static BaseConstantToken? CppTokenToConstantToken(
        CppToken token,
        string compileUnitNamespace,
        ConstLookup lookup
    ) => token.Kind switch
    {
        CppTokenKind.Identifier => new ConstIdentifierToken()
        {
            Value = lookup.Get(token.Text, compileUnitNamespace)
        },
        CppTokenKind.Literal => new ConstantLiteralToken(token.Text),
        CppTokenKind.Punctuation when
            ConstantPunctuationToken.TryParse(token.Text, out var punctuationToken) => punctuationToken,
        _ => null
    };
}
