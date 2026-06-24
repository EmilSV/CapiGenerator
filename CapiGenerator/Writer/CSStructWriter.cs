using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public class CSStructWriter : BaseCSStructWriter
{
    public override async Task Write(CSStruct csStruct, CSWriteConfig writeConfig)
    {
        var structName = csStruct.Name;

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

        await CSTypeDeclarationWriter.WriteStructDeclaration(stream, csStruct);
    }
}
