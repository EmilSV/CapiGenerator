using CapiGenerator.CSModel;
using static CapiGenerator.Writer.StreamWriterUtils;

namespace CapiGenerator.Writer;

public class CSStaticClassWriter : BaseCSStaticClassWriter
{
    public override async Task Write(CSStaticClass csStaticClass, CSWriteConfig writeConfig)
    {
        var staticClassName = csStaticClass.Name;

        using var stream = new StreamWriter(Path.Combine(writeConfig.OutputDirectory, $"{staticClassName}.cs"));

        foreach (var usingNamespace in writeConfig.Usings)
        {
            stream.WriteLine($"using {usingNamespace};");
        }

        await stream.FlushAsync();

        if (csStaticClass.Namespace is not null)
        {
            stream.WriteLine($"namespace {csStaticClass.Namespace};");
        }

        stream.WriteLine($"public static class {staticClassName}");
        stream.WriteLine("{");

        await stream.FlushAsync();

        foreach (var structField in csStaticClass.Fields)
        {
            stream.Write('\t');
            WriteToStream(stream, structField);
            await stream.FlushAsync();
        }

        foreach (var structMethod in csStaticClass.Methods)
        {
            stream.Write('\t');
            WriteToStream(stream, structMethod);
            await stream.FlushAsync();
        }

        stream.WriteLine("}");
    }
}