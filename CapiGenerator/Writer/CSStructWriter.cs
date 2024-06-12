using CapiGenerator.CSModel;
using static CapiGenerator.Writer.StreamWriterUtils;

namespace CapiGenerator.Writer;


public class CSStructWriter : BaseCSStructWriter
{
    public override async Task Write(CSStruct csStruct, CSWriteConfig writeConfig)
    {
        var structName = csStruct.Name;
        var structFields = csStruct.Fields;

        if (writeConfig.OutputDirectory is not null)
        {
            Directory.CreateDirectory(writeConfig.OutputDirectory);
        }
        using var stream = new StreamWriter(Path.Combine(writeConfig.OutputDirectory!, $"{structName}.cs"));

        foreach (var usingNamespace in writeConfig.Usings)
        {
            stream.WriteLine($"using {usingNamespace};");
        }

        await stream.FlushAsync();

        if (csStruct.Namespace is not null)
        {
            stream.WriteLine($"namespace {csStruct.Namespace};");
        }

        stream.Write($"public unsafe ");
        if (csStruct.IsPartial)
        {
            stream.Write("partial ");
        }
        stream.Write($"struct ");
        stream.Write(structName);
        if(csStruct.Interfaces.Count > 0)
        {
            stream.Write(" : ");
            bool first = true;
            foreach (var @interface in csStruct.Interfaces)
            {
                stream.Write($"{@interface}");
                if (!first)
                {
                    stream.Write(", ");
                }
                first = false;
            }
        }
        
        stream.WriteLine();

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