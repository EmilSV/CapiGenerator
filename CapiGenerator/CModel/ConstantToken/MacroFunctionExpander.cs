using CapiGenerator.CModel.BuiltinMacroFunctions;
using CppAst;

namespace CapiGenerator.CModel.ConstantToken;

//TODO: this code is hard to understand and should be refactored
public static class MacroFunctionExpander
{
    public static bool TryExpand(
        IReadOnlyList<CppToken> tokens,
        IReadOnlyDictionary<string, CppMacro> macroFunctions,
        out CppToken[] expandedTokens)
    {
        return new ExpansionContext(macroFunctions).TryExpand(tokens, out expandedTokens);
    }

    private sealed class ExpansionContext(
        IReadOnlyDictionary<string, CppMacro> macroFunctions)
    {
        private readonly HashSet<string> _activeMacros = [];

        public bool TryExpand(
            IReadOnlyList<CppToken> tokens,
            out CppToken[] expandedTokens)
        {
            List<CppToken> output = [];

            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token.Kind == CppTokenKind.Comment)
                {
                    continue;
                }

                if (!TryGetMacroInvocation(tokens, i, out var macro))
                {
                    output.Add(token);
                    continue;
                }

                if (!TryExpandInvocation(tokens, i + 1, macro, out var expansion, out var endIndex))
                {
                    expandedTokens = [];
                    return false;
                }

                output.AddRange(expansion);
                i = endIndex;
            }

            expandedTokens = [.. output];
            return true;
        }

        private bool TryGetMacroInvocation(
            IReadOnlyList<CppToken> tokens,
            int macroIndex,
            out CppMacro macro)
        {
            var token = tokens[macroIndex];
            if (token.Kind != CppTokenKind.Identifier ||
                IsBuiltinMacroFunction(token.Text) ||
                macroIndex + 1 >= tokens.Count ||
                tokens[macroIndex + 1].Text != "(" ||
                !macroFunctions.TryGetValue(token.Text, out var invokedMacro))
            {
                macro = default!;
                return false;
            }

            macro = invokedMacro;
            return true;
        }

        private bool TryExpandInvocation(
            IReadOnlyList<CppToken> tokens,
            int leftParenthesisIndex,
            CppMacro macro,
            out CppToken[] expansion,
            out int endIndex)
        {
            expansion = [];
            if (!TryParseArguments(
                    tokens,
                    leftParenthesisIndex,
                    macro.Parameters.Count,
                    out var arguments,
                    out endIndex) ||
                _activeMacros.Contains(macro.Name))
            {
                return false;
            }

            Dictionary<string, CppToken[]> argumentsByParameter = [];
            for (int i = 0; i < arguments.Count; i++)
            {
                if (!TryExpand(arguments[i], out var expandedArgument))
                {
                    return false;
                }

                argumentsByParameter[macro.Parameters[i]] = expandedArgument;
            }

            var substitutedTokens = SubstituteArguments(macro, argumentsByParameter);

            _activeMacros.Add(macro.Name);
            try
            {
                return TryExpand(substitutedTokens, out expansion);
            }
            finally
            {
                _activeMacros.Remove(macro.Name);
            }
        }

        private static bool TryParseArguments(
            IReadOnlyList<CppToken> tokens,
            int leftParenthesisIndex,
            int parameterCount,
            out List<CppToken[]> arguments,
            out int endIndex)
        {
            arguments = [];
            List<CppToken> currentArgument = [];
            int depth = 0;

            for (int i = leftParenthesisIndex + 1; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token.Text == "(")
                {
                    depth++;
                }
                else if (token.Text == ")")
                {
                    if (depth == 0)
                    {
                        endIndex = i;
                        if (currentArgument.Count > 0 || arguments.Count > 0 || parameterCount > 0)
                        {
                            arguments.Add([.. currentArgument]);
                        }

                        return arguments.Count == parameterCount;
                    }

                    depth--;
                }

                if (token.Text == "," && depth == 0)
                {
                    arguments.Add([.. currentArgument]);
                    currentArgument.Clear();
                }
                else
                {
                    currentArgument.Add(token);
                }
            }

            endIndex = -1;
            return false;
        }

        private static CppToken[] SubstituteArguments(
            CppMacro macro,
            IReadOnlyDictionary<string, CppToken[]> argumentsByParameter)
        {
            List<CppToken> substitutedTokens = [];
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
    }

    private static bool IsBuiltinMacroFunction(string name) =>
        AllBuiltinMacroFunctions.Functions.Any(macroFunction => macroFunction.Name == name);
}
