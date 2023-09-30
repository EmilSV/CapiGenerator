using System.Text;
using CapiGenerator.ConstantToken;
using CapiGenerator.Model;

namespace CapiGenerator;

public class ConstWriter
{
    private readonly IConstantTypeResolver _constantTypeResolver;

    public ConstWriter(IConstantTypeResolver? constantTypeResolver = null)
    {
        _constantTypeResolver = constantTypeResolver ?? new DefaultConstTypeResolver();
    }

    public void WriteToOutFile(CSharpOutFile outputFile, Constant constant, WriterArgs args)
    {
        var name = constant.Output.Name;
        var code = GenerateCode(constant, args);

        outputFile.Add(name, code);
    }

    private string GenerateCode(Constant constant, WriterArgs args)
    {
        StringBuilder builder = new();
        var outputType = constant.ResolveOutputType(_constantTypeResolver);
        var tokens = constant.Output.Tokens.AsSpan();
        var name = constant.Output.Name;


        if (outputType == ConstantType.String)
        {
            GenerateUtf8SpanContent(builder, name, tokens);
        }
        else
        {
            builder.Append("public const ");

            builder.Append(outputType switch
            {
                ConstantType.Int => "long",
                ConstantType.Float => "double",
                ConstantType.Char => "byte",
                _ => throw new ArgumentOutOfRangeException(nameof(outputType), outputType, null)
            });

            builder.Append($" {name} = ");

            foreach (var token in tokens)
            {
                if (token is ConstantLiteralToken literalToken)
                {
                    builder.Append(literalToken.GetOutValue());
                }
                else if (token is ConstantPunctuationToken punctuationToken)
                {
                    builder.Append(punctuationToken.GetOutValue());
                }
                else if (token is ConstIdentifierToken identifierToken)
                {
                    builder.Append(identifierToken.GetOutValue());
                }
                else
                {
                    throw new ArgumentOutOfRangeException(nameof(token), token, null);
                }

                builder.Append(" ");
            }

            builder.Append(";");
        }

        return builder.ToString();
    }

    private void GenerateUtf8SpanContent(StringBuilder builder, string name, ReadOnlySpan<BaseConstantToken> tokens)
    {
        builder.Append($"public static System.ReadOnlySpan<byte> {name} => ");

        foreach (var token in tokens)
        {
            if (token is ConstantLiteralToken literalToken)
            {
                builder.Append(literalToken.GetOutValue());
            }
            else if (token is ConstantPunctuationToken punctuationToken)
            {
                builder.Append(punctuationToken.GetOutValue());
            }
            else if (token is ConstIdentifierToken identifierToken)
            {
                builder.Append(identifierToken.GetOutValue());
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(token), token, null);
            }

            builder.Append(" ");
        }

        builder.Append(";");
    }
}