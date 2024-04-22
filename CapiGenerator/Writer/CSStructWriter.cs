using CapiGenerator.CSModel;
using static CapiGenerator.Writer.StreamWriterUtils;

namespace CapiGenerator.Writer;


public class CSStructWriter : BaseCSStructWriter
{
    public override async Task Write(CSStruct csStruct, CSWriteConfig writeConfig)
    {
        var structName = csStruct.Name;
        var structFields = csStruct.Fields;

        using var stream = new StreamWriter(Path.Combine(writeConfig.OutputDirectory, $"{structName}.cs"));

        foreach (var usingNamespace in writeConfig.Usings)
        {
            stream.WriteLine($"using {usingNamespace};");
        }

        await stream.FlushAsync();

        if (csStruct.Namespace is not null)
        {
            stream.WriteLine($"namespace {csStruct.Namespace};");
        }

        stream.WriteLine($"public unsafe struct {structName}");
        stream.WriteLine("{");

        await stream.FlushAsync();

        foreach (var structField in structFields)
        {
            stream.Write('\t');
            WriteToStream(stream, structField);
            await stream.FlushAsync();
        }

        foreach (var structMethod in csStruct.Methods)
        {
            stream.Write('\t');
            WriteToStream(stream, structMethod);
            await stream.FlushAsync();
        }

        stream.WriteLine("}");

    }
}