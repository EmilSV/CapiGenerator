using CapiGenerator.Parsers;
using CppAst;

namespace CapiGenerator;

public sealed class CapiGeneratorUnit
{
    private readonly CapiModelLookup _modelLookup = new();
    public readonly CapiParser Parser = new();
    public readonly CapiWriter Writer = new();

    public void AddCompilation(CppCompilation compilation, string compileUnitNamespace, CsharpOutFolder outFolder)
    {
        var args = new ParseArgs()
        {
            Compilation = compilation,
            CompileUnitNamespace = compileUnitNamespace,
            Lookups = _modelLookup,
            OutputFolder = outFolder,
        };
        Parser.Parse(args);
    }

    public void AddCompilation(CppCompilation compilation, string compileUnitNamespace, string outFolderPath)
    {
        CsharpOutFolder outFolder = new(outFolderPath);

        var args = new ParseArgs()
        {
            Compilation = compilation,
            CompileUnitNamespace = compileUnitNamespace,
            Lookups = _modelLookup,
            OutputFolder = outFolder,
        };
        Parser.Parse(args);
    }


    public Task WriteToDiskAsync()
    {
        var args = new WriterArgs()
        {
            Lookups = _modelLookup,
        };
        Writer.Write(args);

        HashSet<CSharpOutFile> outFiles = new();

        foreach (var item in _modelLookup.ConstLookup.GetValueCollection())
        {
            outFiles.Add(item.Output.OutputFile);
        }

        return Task.WhenAll(outFiles.Select(x => x.WriteToDiskAsync()).ToArray());
    }



}