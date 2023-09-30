namespace CapiGenerator;

public sealed class CapiWriter
{
    private ConstWriter _defaultConstWriter = new();

    public CapiWriter SetDefaultConstWriter(ConstWriter value)
    {
        _defaultConstWriter = value;
        return this;
    }

    internal void Write(WriterArgs args)
    {
        foreach (var constant in args.Lookups.ConstLookup.GetValueCollection())
        {
            var writer = constant.Output.Writer ?? _defaultConstWriter;
            var outputFile = constant.Output.OutputFile;
            writer.WriteToOutFile(outputFile, constant, args);
        }
    }
}