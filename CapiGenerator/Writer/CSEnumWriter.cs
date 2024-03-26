using System.Text;
using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public class CSEnumWriter : BaseCSEnumWriter
{
    public override async Task Write(CSEnum csEnum, CSWriteConfig writeConfig)
    {
        var enumName = csEnum.Name;
        var enumValues = csEnum.Values.ToArray();

        using var stream = new StreamWriter(Path.Combine(writeConfig.OutputDirectory, $"{enumName}.cs"));

        foreach (var usingNamespace in writeConfig.Usings)
        {
            stream.WriteLine($"using {usingNamespace};");
        }

        await stream.FlushAsync();

        if (csEnum.Namespace is not null)
        {
            stream.WriteLine($"namespace {csEnum.Namespace};");
        }

        stream.WriteLine($"public enum {enumName}");
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
