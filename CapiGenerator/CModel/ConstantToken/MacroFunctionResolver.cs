using CapiGenerator.CModel.BuiltinMacroFunctions;

namespace CapiGenerator.CModel.ConstantToken;

public static class MacroFunctionResolver
{
    public static BaseCConstantToken[] ResolveMacroFunction(BaseCConstantToken[] tokens)
    {
        static List<BaseCConstantToken> ResolveMacroFunctionRecursive(ReadOnlySpan<BaseCConstantToken> tokens)
        {
            List<BaseCConstantToken> output = new();

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] is CConstIdentifierToken identifierToken && identifierToken.TryGetName(out var name))
                {
                    var macroFunction = AllBuiltinMacroFunctions.Functions.FirstOrDefault(mf => mf.Name == name);
                    if (macroFunction is null)
                    {
                        output.Add(tokens[i]);
                        continue;
                    }

                    int leftBracketCount = 0;
                    int rightBracketCount = 0;
                    int endIndex = -1;
                    var start = tokens.Length > i + 2 ? tokens[i + 1] : null;
                    if (start is not CConstantPunctuationToken punctuationToken || punctuationToken.Type != CPunctuationType.LeftParenthesis)
                    {
                        throw new Exception("Expected '(' and ')' after macro function");
                    }

                    for (int j = i + 1; j < tokens.Length; j++)
                    {
                        if (tokens[j] is CConstantPunctuationToken punctuation)
                        {
                            if (punctuation.Type == CPunctuationType.LeftParenthesis)
                            {
                                leftBracketCount++;
                            }
                            else if (punctuation.Type == CPunctuationType.RightParenthesis)
                            {
                                rightBracketCount++;
                            }
                        }

                        if (leftBracketCount > 0 && rightBracketCount > 0 && leftBracketCount == rightBracketCount)
                        {
                            endIndex = j;
                            break;
                        }
                    }
                    if (endIndex == -1)
                    {
                        throw new Exception($"Unmatched parentheses in macro function '{name}'");
                    }

                    int startIndex = i + 2;
                    var arguments = ResolveArguments(tokens.Slice(startIndex, endIndex - startIndex));
                    if (macroFunction.TryEvaluate(arguments, out var result))
                    {
                        output.AddRange(result!);
                    }
                    else
                    {
                        throw new Exception($"Failed to evaluate macro function '{name}' with arguments: {string.Join(", ", arguments.SelectMany(argument => argument))}");
                    }

                    i = endIndex;
                }
                else
                {
                    output.Add(tokens[i]);
                }
            }

            return output;
        }

        static IReadOnlyList<IReadOnlyList<BaseCConstantToken>> ResolveArguments(ReadOnlySpan<BaseCConstantToken> tokens)
        {
            if (tokens.IsEmpty)
            {
                return [];
            }

            List<IReadOnlyList<BaseCConstantToken>> arguments = [];
            int argumentStartIndex = 0;
            int parenthesisDepth = 0;

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] is not CConstantPunctuationToken punctuation)
                {
                    continue;
                }

                switch (punctuation.Type)
                {
                    case CPunctuationType.LeftParenthesis:
                        parenthesisDepth++;
                        break;
                    case CPunctuationType.RightParenthesis:
                        parenthesisDepth--;
                        break;
                    case CPunctuationType.Comma when parenthesisDepth == 0:
                        arguments.Add(ResolveMacroFunctionRecursive(tokens.Slice(argumentStartIndex, i - argumentStartIndex)).ToArray());
                        argumentStartIndex = i + 1;
                        break;
                }
            }

            arguments.Add(ResolveMacroFunctionRecursive(tokens[argumentStartIndex..]).ToArray());
            return arguments;
        }

        return [.. ResolveMacroFunctionRecursive(tokens)];
    }
}
