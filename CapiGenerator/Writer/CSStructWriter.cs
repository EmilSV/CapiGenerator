using System.Diagnostics;
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

        stream.WriteLine();

        await stream.FlushAsync();

        if (csStruct.Namespace is not null)
        {
            stream.WriteLine($"namespace {csStruct.Namespace};");
        }
        stream.WriteLine();

        await WriteToStream(stream, csStruct.Comments);

        stream.Write(csStruct.AccessModifier switch
        {
            CSAccessModifier.Public => "public ",
            CSAccessModifier.Private => "private ",
            CSAccessModifier.Internal => "internal ",
            _ => throw new ArgumentOutOfRangeException("csStruct.AccessModifier.Value")
        });

        if (csStruct.IsUnsafe)
        {
            stream.Write("unsafe ");
        }

        if (csStruct.IsPartial)
        {
            stream.Write("partial ");
        }
        stream.Write($"struct ");
        stream.Write(structName);
        if (csStruct.Interfaces.Count > 0)
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
            if(structName == "TextureDescriptorFFI" && structField.Name == "ViewFormats")
            {
                Debugger.Break();
            }

            await WriteToStream(stream, structField.Comments);
            stream.Write('\t');
            WriteToStream(stream, structField);
            await stream.FlushAsync();
        }

        foreach (var constructor in csStruct.Constructors)
        {
            //await WriteToStream(stream, constructor.Comments);
            stream.Write('\t');
            await WriteToStream(stream, constructor);
            await stream.FlushAsync();
        }


        foreach (var structMethod in csStruct.Methods)
        {
            await WriteToStream(stream, structMethod.Comments);
            stream.Write('\t');
            await WriteToStream(stream, structMethod);
            await stream.FlushAsync();
        }

        stream.WriteLine("}");

    }
}