using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel.BuiltinMacroFunctions;
using CppAst;

namespace CapiGenerator.CModel.ConstantToken;

public static class MacroFunctionExpander
{
    public static bool TryExpand(
        IReadOnlyList<CppToken> tokens,
        IReadOnlyDictionary<string, CppMacro> macroFunctions,
        [NotNullWhen(true)] out CppToken[]? expandedTokens)
    {
        try
        {
            var macroFunctionContext = new MacroFunctionContext(macroFunctions);
            expandedTokens = [.. macroFunctionContext.ExpandAllMacroFunctions(tokens)];
        }
        catch (MacroFunctionExpanderException ex)
        {
            Console.Error.WriteLine(ex.Message);
            expandedTokens = default;
            return false;
        }
        return true;
    }
}

file readonly struct MacroFunctionContext(IReadOnlyDictionary<string, CppMacro> macroFunctions)
{
    private readonly HashSet<string> _activeMacroFunctions = [];

    private bool TryExpandNextMarcoFunction(List<CppToken> tokens, out MacroExpandResult expandResult)
    {
        CppMacro? invokedMacro = null;
        var count = tokens.Count;
        int i;
        for (i = 0; i < count - 1; i++)
        {
            var token = tokens[i];
            var nextToken = tokens[i + 1];
            if (token.Kind != CppTokenKind.Identifier)
            {
                continue;
            }

            if (nextToken.Kind != CppTokenKind.Punctuation || nextToken.Text != "(")
            {
                continue;
            }

            var macroName = token.Text;

            if (IsBuiltinMacroFunction(macroName))
            {
                continue;
            }

            if (!macroFunctions.TryGetValue(macroName, out invokedMacro))
            {
                throw new UnknownMacroException(macroName);
            }

            break;
        }
        var macroFunctionStartIndex = i;

        if (invokedMacro is null)
        {
            expandResult = default;
            return false;
        }

        var openParenCount = 0;

        i = macroFunctionStartIndex + 1;

        //Find end of macro function invocation
        for (; i < count; i++)
        {
            var token = tokens[i];
            if (token.Kind == CppTokenKind.Punctuation && token.Text == "(")
            {
                openParenCount++;
                continue;
            }

            if (token.Kind == CppTokenKind.Punctuation && token.Text == ")")
            {
                openParenCount--;
            }
            else
            {
                continue;
            }

            if (openParenCount == 0)
            {
                break;
            }
        }
        if (openParenCount != 0)
        {
            throw new MalformedMacroCallException();
        }

        var macroEndFunctionIndex = i;

        //find arguments
        var arguments = new List<List<CppToken>>();
        var currentArgument = new List<CppToken>();

        openParenCount = 0;

        for (int j = macroFunctionStartIndex + 2; j < macroEndFunctionIndex; j++)
        {
            var token = tokens[j];
            if (token.Kind == CppTokenKind.Punctuation && token.Text == "(")
            {
                openParenCount++;
            }
            else if (token.Kind == CppTokenKind.Punctuation && token.Text == ")")
            {
                openParenCount--;
            }
            else if (token.Kind == CppTokenKind.Punctuation && token.Text == "," && openParenCount == 0)
            {
                arguments.Add(currentArgument);
                currentArgument = [];
                continue;
            }

            currentArgument.Add(token);
        }
        var invocationIsEmpty =
            macroFunctionStartIndex + 2 == macroEndFunctionIndex;

        if (!invocationIsEmpty || invokedMacro.Parameters.Count > 0)
        {
            arguments.Add(currentArgument);
        }

        List<CppToken> outputTokens = ExpandMarcoFunction(invokedMacro, arguments);

        expandResult = new MacroExpandResult
        {
            StartIndex = macroFunctionStartIndex,
            EndIndex = macroEndFunctionIndex,
            OutputTokens = outputTokens,
        };

        return true;
    }

    private List<CppToken> ExpandMarcoFunction(
        CppMacro macroFunction,
        List<List<CppToken>> arguments)
    {
        if (macroFunction.Parameters.Count != arguments.Count)
        {
            throw new MalformedMacroCallException();
        }

        List<List<CppToken>> expandedArguments = [];
        foreach (var argument in arguments)
        {
            expandedArguments.Add(ExpandAllMacroFunctions(argument));
        }

        if (!_activeMacroFunctions.Add(macroFunction.Name))
        {
            throw new RecursiveMacroFunctionException(macroFunction.Name);
        }

        try
        {
            var substitutedTokens =
                SubstituteArguments(macroFunction, expandedArguments);

            return ExpandAllMacroFunctions(substitutedTokens);
        }
        finally
        {
            _activeMacroFunctions.Remove(macroFunction.Name);
        }
    }

    private static CppToken[] SubstituteArguments(
        CppMacro macro,
        List<List<CppToken>> arguments)
    {
        if (macro.Parameters.Count != arguments.Count)
        {
            throw new MalformedMacroCallException();
        }

        List<CppToken> substitutedTokens = [];
        Dictionary<string, List<CppToken>> argumentsByParameter = arguments
            .Select((arg, i) => (Name: macro.Parameters[i], arg))
            .ToDictionary(pair => pair.Name, pair => pair.arg);

        foreach (var token in macro.Tokens)
        {
            if (token.Kind == CppTokenKind.Identifier &&
                argumentsByParameter.TryGetValue(token.Text, out var argument))
            {
                substitutedTokens.AddRange(argument);
            }
            else
            {
                substitutedTokens.Add(token);
            }
        }

        return [.. substitutedTokens];
    }

    public List<CppToken> ExpandAllMacroFunctions(
        IReadOnlyList<CppToken> tokens)
    {
        List<CppToken> expandedTokens = [.. tokens];

        while (TryExpandNextMarcoFunction(expandedTokens, out var macroFunction))
        {
            var startIndex = macroFunction.StartIndex;
            var endIndex = macroFunction.EndIndex;
            var output = macroFunction.OutputTokens;
            var removalCount = endIndex - startIndex + 1;
            expandedTokens.RemoveRange(startIndex, removalCount);
            expandedTokens.InsertRange(startIndex, output);
        }

        return expandedTokens;
    }

    private static bool IsBuiltinMacroFunction(string name) =>
        AllBuiltinMacroFunctions.Functions.Any(macroFunction => macroFunction.Name == name);
}


file abstract class MacroFunctionExpanderException : Exception
{

}

file class UnknownMacroException(string macroName) : MacroFunctionExpanderException
{
    public string MacroName => macroName;

    public override string Message => $"Unknown macro function: {macroName}";
}

file class MalformedMacroCallException : MacroFunctionExpanderException
{
    public override string Message => "Malformed macro function";
}

file class RecursiveMacroFunctionException(string macroName) : MacroFunctionExpanderException
{
    public string MacroName => macroName;

    public override string Message => $"Recursive macro function: {macroName}";
}

file readonly struct MacroExpandResult
{
    public int StartIndex { get; init; }
    public int EndIndex { get; init; }
    public List<CppToken> OutputTokens { get; init; }
}
