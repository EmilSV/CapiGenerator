using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel.BuiltinMacroFunctions;
using CppAst;

namespace CapiGenerator.CModel.ConstantToken;


public static class MacroFunctionExpander
{
    private readonly struct MacroExpandResult
    {
        public int StartIndex { get; init; }
        public int EndIndex { get; init; }
        public List<CppToken> OutputTokens { get; init; }
    }

    private abstract class MacroFunctionExpanderException : Exception
    {

    }

    private class UnknownMacroException(string macroName) : MacroFunctionExpanderException
    {
        public string MacroName => macroName;
    }

    private class MalformedMacroException : MacroFunctionExpanderException
    {

    }
    public static bool TryExpand(
        IReadOnlyList<CppToken> tokens,
        IReadOnlyDictionary<string, CppMacro> macroFunctions,
        [NotNullWhen(true)] out CppToken[]? expandedTokens)
    {
        try
        {

            expandedTokens = [.. ExpandAllMacroFunctions(tokens, macroFunctions)];
        }
        catch (MacroFunctionExpanderException ex)
        {
            Console.WriteLine(ex.Message);
            expandedTokens = default;
            return false;
        }
        return true;
    }

    private static bool TryExpandNextMarcoFunction(List<CppToken> tokens, IReadOnlyDictionary<string, CppMacro> macroFunctions, out MacroExpandResult expandResult)
    {
        CppMacro? invokedMacro = null;
        var count = tokens.Count - 1;
        int i;
        for (i = 0; i < count; i++)
        {
            var token = tokens[i];
            var nextToken = tokens[i + 1];
            if (token.Kind != CppTokenKind.Identifier)
            {
                continue;
            }

            if (nextToken.Kind != CppTokenKind.Literal && nextToken.Text != "(")
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

        var openParenCount = 1;

        //Find end of macro function invocation
        for (i = 0; i < count; i++)
        {
            var token = tokens[i];
            if (token.Kind == CppTokenKind.Literal && token.Text == "(")
            {
                openParenCount++;
                continue;
            }

            if (token.Kind == CppTokenKind.Literal && token.Text == ")")
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
            throw new MalformedMacroException();
        }

        var macroEndFunctionIndex = i;
        var output = new List<CppToken>();

        //find arguments
        var arguments = new List<List<CppToken>>();
        var currentArgument = new List<CppToken>();

        openParenCount = 0;

        for (int j = macroFunctionStartIndex + 2; j < macroEndFunctionIndex; j++)
        {
            var token = tokens[j];
            if (token.Kind == CppTokenKind.Literal && token.Text == "(")
            {
                openParenCount++;
                continue;
            }
            else if (token.Kind == CppTokenKind.Literal && token.Text == ")")
            {
                openParenCount--;
            }
            else if (token.Kind == CppTokenKind.Literal && token.Text == "," && openParenCount == 0)
            {
                arguments.Add(currentArgument);
                currentArgument = [];
                continue;
            }

            currentArgument.Add(token);
        }
        arguments.Add(currentArgument);

        List<CppToken> outputTokens = ExpandMarcoFunction(invokedMacro, macroFunctions, arguments);

        expandResult = new MacroExpandResult
        {
            StartIndex = macroFunctionStartIndex,
            EndIndex = macroEndFunctionIndex,
            OutputTokens = outputTokens,
        };

        return true;
    }

    private static List<CppToken> ExpandMarcoFunction(
        CppMacro macroFunction,
        IReadOnlyDictionary<string, CppMacro> macroFunctions,
        List<List<CppToken>> arguments)
    {
        List<CppToken> expandedTokens = [.. SubstituteArguments(macroFunction, arguments)];
        return ExpandAllMacroFunctions(expandedTokens, macroFunctions);
    }

    private static CppToken[] SubstituteArguments(
        CppMacro macro,
        List<List<CppToken>> arguments)
    {
        if (macro.Parameters.Count != arguments.Count)
        {
            throw new MalformedMacroException();
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

    private static List<CppToken> ExpandAllMacroFunctions(
        IReadOnlyList<CppToken> tokens,
        IReadOnlyDictionary<string, CppMacro> macroFunctions)
    {
        List<CppToken> expandedTokens = [.. tokens];

        while (TryExpandNextMarcoFunction(expandedTokens, macroFunctions, out var macroFunction))
        {
            var startIndex = macroFunction.StartIndex;
            var endIndex = macroFunction.EndIndex;
            var output = macroFunction.OutputTokens;
            expandedTokens.RemoveRange(startIndex, endIndex - startIndex);
            expandedTokens.InsertRange(startIndex, output);
        }

        return expandedTokens;
    }

    private static bool IsBuiltinMacroFunction(string name) =>
        AllBuiltinMacroFunctions.Functions.Any(macroFunction => macroFunction.Name == name);
}


//TODO: this code is hard to understand and should be refactored
// public static class MacroFunctionExpander
// {
//     public static bool TryExpand(
//         IReadOnlyList<CppToken> tokens,
//         IReadOnlyDictionary<string, CppMacro> macroFunctions,
//         out CppToken[] expandedTokens)
//     {
//         HashSet<string> activeMacro = [];
//         return DoTryExpand(tokens, activeMacro, macroFunctions, out expandedTokens);
//     }

//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     private static bool DoTryExpand(
//         IReadOnlyList<CppToken> tokens,
//         HashSet<string> activeMacro,
//         IReadOnlyDictionary<string, CppMacro> macroFunctions,
//         out CppToken[] expandedTokens)
//     {
//         List<CppToken> output = [];

//         for (int i = 0; i < tokens.Count; i++)
//         {
//             var token = tokens[i];
//             if (token.Kind == CppTokenKind.Comment)
//             {
//                 continue;
//             }

//             if (!TryGetMacroInvocation(tokens, i, macroFunctions, out var macro))
//             {
//                 output.Add(token);
//                 continue;
//             }

//             if (!TryExpandInvocation(tokens, i + 1, macro, activeMacro, macroFunctions, out var expansion, out var endIndex))
//             {
//                 expandedTokens = [];
//                 return false;
//             }

//             output.AddRange(expansion);
//             i = endIndex;
//         }

//         expandedTokens = [.. output];
//         return true;
//     }

//     private static bool TryGetMacroInvocation(
//         IReadOnlyList<CppToken> tokens,
//         int macroIndex,
//         IReadOnlyDictionary<string, CppMacro> macroFunctions,
//         out CppMacro macro)
//     {
//         var token = tokens[macroIndex];
//         if (token.Kind != CppTokenKind.Identifier ||
//             IsBuiltinMacroFunction(token.Text) ||
//             macroIndex + 1 >= tokens.Count ||
//             tokens[macroIndex + 1].Text != "(\"" ||
//             !macroFunctions.TryGetValue(token.Text, out var invokedMacro))
//         {
//             macro = default!;
//             return false;
//         }

//         macro = invokedMacro;
//         return true;
//     }

//     private static bool TryExpandInvocation(
//         IReadOnlyList<CppToken> tokens,
//         int leftParenthesisIndex,
//         CppMacro macro,
//         HashSet<string> activeMacros,
//         IReadOnlyDictionary<string, CppMacro> macroFunctions,
//         out CppToken[] expansion,
//         out int endIndex)
//     {
//         expansion = [];
//         if (!TryParseArguments(
//                 tokens,
//                 leftParenthesisIndex,
//                 macro.Parameters.Count,
//                 out var arguments,
//                 out endIndex) ||
//             activeMacros.Contains(macro.Name))
//         {
//             return false;
//         }

//         Dictionary<string, CppToken[]> argumentsByParameter = [];
//         for (int i = 0; i < arguments.Count; i++)
//         {
//             if (!DoTryExpand(arguments[i], activeMacros, macroFunctions, out var expandedArgument))
//             {
//                 return false;
//             }

//             argumentsByParameter[macro.Parameters[i]] = expandedArgument;
//         }

//         var substitutedTokens = SubstituteArguments(macro, argumentsByParameter);

//         activeMacros.Add(macro.Name);
//         try
//         {
//             return DoTryExpand(substitutedTokens, activeMacros, macroFunctions, out expansion);
//         }
//         finally
//         {
//             activeMacros.Remove(macro.Name);
//         }
//     }

//     private static CppToken[] SubstituteArguments(
//         CppMacro macro,
//         IReadOnlyDictionary<string, CppToken[]> argumentsByParameter)
//     {
//         List<CppToken> substitutedTokens = [];
//         foreach (var token in macro.Tokens)
//         {
//             if (token.Kind == CppTokenKind.Identifier &&
//                 argumentsByParameter.TryGetValue(token.Text, out var argument))
//             {
//                 substitutedTokens.AddRange(argument);
//             }
//             else
//             {
//                 substitutedTokens.Add(token);
//             }
//         }

//         return [.. substitutedTokens];
//     }


//     private static bool TryParseArguments(
//         IReadOnlyList<CppToken> tokens,
//         int leftParenthesisIndex,
//         int parameterCount,
//         out List<CppToken[]> arguments,
//         out int endIndex)
//     {
//         arguments = [];
//         List<CppToken> currentArgument = [];
//         int depth = 0;

//         for (int i = leftParenthesisIndex + 1; i < tokens.Count; i++)
//         {
//             var token = tokens[i];
//             if (token.Text == "(")
//             {
//                 depth++;
//             }
//             else if (token.Text == ")")
//             {
//                 if (depth == 0)
//                 {
//                     endIndex = i;
//                     if (currentArgument.Count > 0 || arguments.Count > 0 || parameterCount > 0)
//                     {
//                         arguments.Add([.. currentArgument]);
//                     }

//                     return arguments.Count == parameterCount;
//                 }

//                 depth--;
//             }

//             if (token.Text == "," && depth == 0)
//             {
//                 arguments.Add([.. currentArgument]);
//                 currentArgument.Clear();
//             }
//             else
//             {
//                 currentArgument.Add(token);
//             }
//         }

//         endIndex = -1;
//         return false;
//     }

//     private static bool IsBuiltinMacroFunction(string name) =>
//         AllBuiltinMacroFunctions.Functions.Any(macroFunction => macroFunction.Name == name);
// }
