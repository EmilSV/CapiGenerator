using System.Text;
using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public class CSEnumWriter : BaseCSEnumWriter
{
    public override async Task Write(CSEnum csEnum, CSWriteConfig writeConfig)
    {
        var enumName = csEnum.Name;
        var enumValues = csEnum.Values;

        if (writeConfig.OutputDirectory is not null)
        {
            Directory.CreateDirectory(writeConfig.OutputDirectory);
        }

        using var stream = new StreamWriter(Path.Combine(writeConfig.OutputDirectory!, $"{enumName}.cs"));

        foreach (var usingNamespace in writeConfig.Usings)
        {
            stream.WriteLine($"using {usingNamespace};");
        }
        stream.WriteLine();

        await stream.FlushAsync();

        if (csEnum.Namespace is not null)
        {
            stream.WriteLine($"namespace {csEnum.Namespace};");
        }

        stream.WriteLine();

        foreach (var attribute in csEnum.Attributes)
        {
            StreamWriterUtils.WriteToStream(stream, attribute);
            stream.WriteLine();
        }

        await stream.FlushAsync();

        stream.Write($"public enum {enumName}");
        if (csEnum.Type.KindValue != CSPrimitiveType.Kind.Int)
        {
            stream.Write($" : {csEnum.Type.Name}");
        }
        stream.WriteLine();
        stream.WriteLine("{");

        await stream.FlushAsync();

        foreach (var enumValue in enumValues)
        {
            stream.WriteLine($"\t{enumValue.Name} = {enumValue.Expression},");
            await stream.FlushAsync();
        }

        stream.WriteLine("}");
        await stream.FlushAsync();
    }
}
