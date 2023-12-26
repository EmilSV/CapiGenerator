using CapiGenerator.ConstantToken;
using CapiGenerator.Model;
using CppAst;

namespace CapiGenerator.OutputGenerator;


public class ConstantOutputGenerator
{
    public Constant.ConstantOutput? GeneratorOutput(
        Constant constant, GeneratorOutputArgs args)
    {
        var inputTokens = constant.Input.Macro.Tokens;
        var length = inputTokens.Count;
        var outputTokens = new ConstantToken.BaseConstantToken?[length];
        for (int i = 0; i < length; i++)
        {
            var inputToken = constant.Input.Macro.Tokens[i];
            var outputToken = GeneratorOutputToken(constant, inputToken, args);
            if (outputToken is null)
            {
                break;
            }
        }

        if (outputTokens.Any(t => t is null))
        {
            return null;
        }

        var namespaceName = DllNameToNamespace(constant.Input.DllName);

        var inputMacro = constant.Input.Macro;


        var output = new Constant.ConstantOutput
        {
            Name = constant.Input.Name,
            Namespace = namespaceName,
            ClassName = namespaceName + "Consts",
            Comment = constant.Input.Macro.,
            Tokens = outputTokens.Select(t => t!).ToImmutableArray(),
            Writer = constant.Output?.Writer
        };
    }

    private string DllNameToNamespace(string dllName)
    {
        return char.ToUpper(dllName[0]) + dllName[1..] + "FFI";
    }

    private BaseConstantToken? GeneratorOutputToken(
        Constant constant, CppToken inputToken, GeneratorOutputArgs args)
    {
        switch (inputToken.Kind)
        {
            case CppTokenKind.Literal: return new ConstantLiteralToken(inputToken.Text);
            case CppTokenKind.Identifier:
                var identifier = inputToken.Text;
                var dllName = constant.Input.DllName;
                var identifierConstant = args.OtherConstants.FirstOrDefault(
                    c => c.Input.Name == identifier || c.Input.DllName == dllName
                );
                if (identifierConstant is null)
                {
                    return null;
                }
                else
                {
                    return new ConstIdentifierToken(identifierConstant);
                }

            case CppTokenKind.Punctuation:
                if (ConstantPunctuationToken.TryParse(inputToken.Text, out var punctuationToken))
                {
                    return punctuationToken;
                }
                else
                {
                    return null;
                }
            default: return null;
        }
    }
}