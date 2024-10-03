using CapiGenerator.CSModel;
using static CapiGenerator.Writer.StreamWriterUtils;

namespace CapiGenerator.Writer;

public class CSStaticClassWriter : BaseCSStaticClassWriter
{
    public override async Task Write(CSStaticClass csStaticClass, CSWriteConfig writeConfig)
    {
        var staticClassName = csStaticClass.Name;

        string outputDirectory = writeConfig.OutputDirectory;

        Directory.CreateDirectory(outputDirectory);

        var filePath = Path.Combine(outputDirectory, $"{staticClassName}.cs");

        var directoryPath = Path.GetDirectoryName(filePath);

        if (directoryPath is not null)
        {
            Directory.CreateDirectory(directoryPath);
        }
        using var stream = new StreamWriter(Path.Combine(outputDirectory, $"{staticClassName}.cs"));

        foreach (var usingNamespace in writeConfig.Usings)
        {
            stream.WriteLine($"using {usingNamespace};");
        }

        stream.WriteLine();

        await stream.FlushAsync();

        if (csStaticClass.Namespace is not null)
        {
            stream.WriteLine($"namespace {csStaticClass.Namespace};");
        }

        stream.WriteLine();

        await WriteToStream(stream, csStaticClass.Comments);
        stream.Write($"public unsafe static");
        if (csStaticClass.IsPartial)
        {
            stream.Write(" partial");
        }
        stream.Write($" class ");
        stream.WriteLine(staticClassName);
        stream.WriteLine("{");

        await stream.FlushAsync();

        foreach (var structField in csStaticClass.Fields)
        {
            await WriteToStream(stream, structField.Comments);
            stream.Write('\t');
            WriteToStream(stream, structField);
            await stream.FlushAsync();
        }

        stream.WriteLine();

        foreach (var structMethod in csStaticClass.Methods)
        {
            await WriteToStream(stream, structMethod.Comments);
            stream.Write('\t');
            await WriteToStream(stream, structMethod);
            await stream.FlushAsync();
        }

        stream.WriteLine("}");
    }
}