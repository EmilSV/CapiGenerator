using CapiGenerator.CSModel;

namespace CapiGenerator.Writer;

public class CSEnumWriter : BaseCSEnumWriter
{
    public override async Task Write(CSEnum csEnum, CSWriteConfig writeConfig)
    {
        var enumName = csEnum.Name;

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

        await CSTypeDeclarationWriter.WriteEnumDeclaration(stream, csEnum);
    }
}
