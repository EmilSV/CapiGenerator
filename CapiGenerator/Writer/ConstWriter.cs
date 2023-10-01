using System.Text;
using CapiGenerator.ConstantToken;
using CapiGenerator.Model;
using CapiGenerator.OutFile;

namespace CapiGenerator;

public class ConstWriter
{
    private readonly IConstantTypeResolver _constantTypeResolver;

    public ConstWriter(IConstantTypeResolver? constantTypeResolver = null)
    {
        _constantTypeResolver = constantTypeResolver ?? new DefaultConstTypeResolver();
    }

    public void WriteToOutFile(BaseCSharpOutFile outputFile, Constant constant, WriterArgs args)
    {
        var name = constant.Output.Name;
        var code = GenerateCode(constant, args);

        if (outputFile is StructCSharpOutFile structOutFile)
        {
            structOutFile.Add(name, code);
        }
        else if (outputFile is StaticClassCSharpOutFile staticClassOutFile)
        {
            staticClassOutFile.Add(name, code);
        }
        else if (outputFile is EnumCSharpOutFile)
        {
            throw new NotSupportedException("Enums are not supported");
        }
        else if (outputFile is ClassCSharpOutFile classOutFile)
        {
            classOutFile.Add(name, code);
        }
    }

    private string GenerateCode(Constant constant, WriterArgs args)
    {
        StringBuilder builder = new();
        var outputType = constant.ResolveOutputType(_constantTypeResolver);
        var tokens = constant.Output.Tokens.AsSpan();
        var name = constant.Output.Name;
        var constLookup = args.Lookups.ConstLookup;


        if (outputType == ConstantType.String)
        {
            GenerateUtf8SpanContent(builder, name, tokens, args);
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
                builder.Append(token.GetOutValue(constLookup));
            }

            builder.Append(";");
        }

        return builder.ToString();
    }

    private void GenerateUtf8SpanContent(
        StringBuilder builder,
        string name,
        ReadOnlySpan<BaseConstantToken> tokens,
        WriterArgs args)
    {
        var constLookup = args.Lookups.ConstLookup;
        builder.Append($"public static System.ReadOnlySpan<byte> {name} => ");

        foreach (var token in tokens)
        {
            builder.Append(token.GetOutValue(constLookup) + " ");
        }

        builder.Append(";");
    }
}