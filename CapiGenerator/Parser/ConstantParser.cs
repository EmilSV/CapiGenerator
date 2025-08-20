using System.Diagnostics;
using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.CModel.BuiltinMacroFunctions;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CppAst;

namespace CapiGenerator.Parser;


public class ConstantParser : BaseParser
{
    public override void FirstPass(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel)
    {
        foreach (var compilation in compilations)
        {
            foreach (var macro in compilation.Macros)
            {
                if (macro.Parameters != null && macro.Parameters.Count > 0)
                {
                    continue;
                }

                if (macro.Tokens.Count == 0)
                {
                    continue;
                }

                if (ShouldSkip(macro))
                {
                    continue;
                }

                var builtinConstant = AllBuiltinCConstants.AllCConstantTypes.FirstOrDefault(bc => bc.MacroIsBuiltin(macro));
                if (builtinConstant is not null)
                {
                    outputChannel.OnReceiveConstant(builtinConstant);
                    continue;
                }

                var newConst = FirstPass(macro);
                if (newConst is not null)
                {
                    outputChannel.OnReceiveConstant(newConst);
                }
            }

            foreach (var field in compilation.Fields)
            {
                if (field.StorageQualifier != CppStorageQualifier.Static)
                {
                    continue;
                }

                if (field.Type is not CppQualifiedType type || type.Qualifier != CppTypeQualifier.Const)
                {
                    continue;
                }

                if (ShouldSkip(field))
                {
                    continue;
                }

                var newConstant = FirstPass(field);
                if (newConstant is not null)
                {
                    outputChannel.OnReceiveConstant(newConstant);
                }
            }
        }
    }

    public override void SecondPass(CCompilationUnit compilationUnit, BaseParserInputChannel inputChannel)
    {
        foreach (var constant in inputChannel.GetConstants())
        {
            SecondPass(constant, compilationUnit);
        }
    }

    protected virtual void SecondPass(BaseCConstant value, CCompilationUnit compilationUnit)
    {
        value.OnSecondPass(compilationUnit);
    }

    protected virtual CConstant? FirstPass(CppMacro macro)
    {
        var constantTokens = macro.Tokens.Select(CppTokenToConstantToken).ToArray();
        if (constantTokens == null || constantTokens.Any(token => token is null))
        {
            OnError(macro, "Failed to parse tokens");
            return null;
        }

        constantTokens = ResolveMacroFunction(constantTokens!);

        return new(macro.Name, new(constantTokens!));
    }

    protected virtual CStaticConstant? FirstPass(CppField field)
    {
        var constantType = CppTypeToConstantType(field.Type);
        if (constantType is null)
        {
            return null;
        }

        return new(
            realType: CTypeInstance.FromCppType(field.Type),
            constantType: constantType.Value,
            name: field.Name,
            expression: [new CConstLiteralToken(field.InitValue.Value!.ToString()!)]
        );
    }

    protected virtual BaseCConstantToken[] ResolveMacroFunction(BaseCConstantToken[] tokens)
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
                    var resolvedTokens = ResolveMacroFunctionRecursive(tokens.Slice(startIndex, endIndex - startIndex));
                    if (macroFunction.TryEvaluate(CollectionsMarshal.AsSpan(resolvedTokens), out var result))
                    {
                        output.AddRange(result!);
                    }
                    else
                    {
                        throw new Exception($"Failed to evaluate macro function '{name}' with arguments: {string.Join(", ", resolvedTokens)}");
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

        return ResolveMacroFunctionRecursive(tokens).ToArray();
    }

    protected virtual bool ShouldSkip(CppMacro constant) => false;
    protected virtual bool ShouldSkip(CppField constant) => false;
    protected virtual void OnError(CppMacro macro, string message)
    {
        Console.Error.WriteLine($"Error parsing constant {macro.Name}: {message}");
    }

    protected virtual void OnError(CppField field, string message)
    {
        Console.Error.WriteLine($"Error parsing constant {field.Name}: {message}");
    }


    public static CConstantType? CppTypeToConstantType(CppType type)
    {
        static CppPrimitiveKind? GetPrimitiveType(CppType type)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return primitiveType.Kind;
            }
            if (type is CppTypedef typedef)
            {
                return GetPrimitiveType(typedef.ElementType);
            }
            if (type is CppQualifiedType qualifiedType && qualifiedType.Qualifier == CppTypeQualifier.Const)
            {
                return GetPrimitiveType(qualifiedType.ElementType);
            }
            return null;
        }

        var primitiveType = GetPrimitiveType(type);
        if (primitiveType is null)
        {
            return null;
        }

        return primitiveType switch
        {
            CppPrimitiveKind.Char => CConstantType.Char,
            CppPrimitiveKind.Int => CConstantType.Int,
            CppPrimitiveKind.UnsignedInt => CConstantType.UnsignedInt,
            CppPrimitiveKind.LongLong => CConstantType.LongLong,
            CppPrimitiveKind.UnsignedLongLong => CConstantType.UnsignedLongLong,
            CppPrimitiveKind.Float => CConstantType.Float,
            CppPrimitiveKind.Double => CConstantType.Double,
            CppPrimitiveKind.Short => CConstantType.Short,
            CppPrimitiveKind.Long => CConstantType.Long,
            CppPrimitiveKind.UnsignedChar => CConstantType.UnsignedChar,
            CppPrimitiveKind.UnsignedShort => CConstantType.UnsignedShort,
            CppPrimitiveKind.UnsignedLong => CConstantType.UnsignedLong,
            _ => null
        };
    }

    public static BaseCConstantToken? CppTokenToConstantToken(CppToken token) => token.Kind switch
    {
        CppTokenKind.Identifier => new CConstIdentifierToken(token.Text),
        CppTokenKind.Literal => new CConstLiteralToken(token.Text),
        CppTokenKind.Punctuation when
            CConstantPunctuationToken.TryParse(token.Text, out var punctuationToken) => punctuationToken,
        _ => null
    };
}