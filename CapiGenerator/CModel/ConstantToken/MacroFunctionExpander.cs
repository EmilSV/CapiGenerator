using System.Collections;
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
        if (!TryFindStartOfMacroFunction(tokens, out int macroFunctionStartIndex, out CppMacro? invokedMacro))
        {
            expandResult = default;
            return false;
        }

        var tokenFromFunctionStartingParam = new ReadonlyListSpan<CppToken>(tokens, macroFunctionStartIndex + 1);
        var functionTokensWithoutName = SliceToEndOfMacroFunction(tokenFromFunctionStartingParam);

        var macroFunctionEndIndex = functionTokensWithoutName.MapToOriginalIndex(functionTokensWithoutName.Count - 1);

        var tokenInsideParentheses = functionTokensWithoutName[1..^1];
        var arguments = ExtractMacroFunctionArguments(tokenInsideParentheses, invokedMacro);

        List<CppToken> outputTokens = ExpandMarcoFunction(invokedMacro, arguments);

        expandResult = new MacroExpandResult
        {
            StartIndex = macroFunctionStartIndex,
            EndIndex = macroFunctionEndIndex,
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

    private bool TryFindStartOfMacroFunction(ReadonlyListSpan<CppToken> tokens, out int startIndex, [NotNullWhen(true)] out CppMacro? invokedMacro)
    {
        var count = tokens.Count;
        int i;
        invokedMacro = null;
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
        startIndex = i;
        if (invokedMacro is null)
        {
            return false;
        }
        return true;
    }

    private static ReadonlyListSpan<CppToken> SliceToEndOfMacroFunction(ReadonlyListSpan<CppToken> tokens)
    {
        var openParenCount = 0;
        int i = 0;
        for (; i < tokens.Count; i++)
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

        return tokens[..(i + 1)];
    }

    private static List<List<CppToken>> ExtractMacroFunctionArguments(ReadonlyListSpan<CppToken> tokens, CppMacro invokedMacro)
    {
        var arguments = new List<List<CppToken>>();
        var currentArgument = new List<CppToken>();

        var openParenCount = 0;

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
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
        if (tokens.Count > 0 || invokedMacro.Parameters.Count > 0)
        {
            arguments.Add(currentArgument);
        }

        return arguments;
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

file readonly struct ReadonlyListSpan<T> : IReadOnlyList<T>
{
    private readonly List<T> _list;
    private readonly int _start;
    private readonly int _length;

    public ReadonlyListSpan(List<T> list, int start)
    {
        if (start < 0 || start > list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(list), "Index out of range");
        }

        this._list = list;
        this._start = start;
        this._length = list.Count - start;
    }

    public ReadonlyListSpan(List<T> list, int start, int length)
    {
        if (start < 0 || length < 0 || start + length > list.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(list), "Index out of range");
        }

        this._list = list;
        this._start = start;
        this._length = length;
    }

    public ReadonlyListSpan(ReadonlyListSpan<T> span, int start, int length)
    {
        if (start < 0 || length < 0 || start + length > span.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(span), "Index out of range");
        }

        this._list = span._list;
        this._start = span._start + start;
        this._length = length;
    }

    public T this[int index]
    {
        get
        {
            if (index < 0 || index >= _length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range");
            }
            return _list[_start + index];
        }
    }

    public static implicit operator ReadonlyListSpan<T>(List<T> list) => new(list, 0, list.Count);

    public int Count => _length;

    public ReadonlyListSpan<T> Slice(int start, int length) => new(this, start, length);
    public ReadonlyListSpan<T> Slice(int start) => new(this, start, _length - start);

    public int MapToOriginalIndex(int index) => _start + index;

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _length; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
