using CapiGenerator.Model;
using CapiGenerator.OutFile;
using CapiGenerator.Parser;
using CppAst;
using System.Reflection.Emit;

namespace CapiGenerator;

public sealed class CapiGeneratorUnit
{
    public readonly CapiFactory Factory = new();
    public readonly CapiParser Parser = new();
    public readonly CapiWriter Writer = new();

    private Dictionary<string, CsharpOutFolder> _outFolders = new();


    public void AddCompilation(CppCompilation compilation, string @namespace, string outFolderPath)
    {
        outFolderPath = Path.GetFullPath(outFolderPath);
        if (_outFolders.TryGetValue(outFolderPath, out var outFolder))
        {
            if (outFolder.Namespace != @namespace)
            {
                throw new Exception($"Out folder {outFolderPath} is already used for namespace {outFolder.Namespace}.");
            }
        }
        else
        {
            outFolder = new(outFolderPath, @namespace);
            _outFolders.Add(outFolderPath, outFolder);
        }

        var args = new ParseArgs()
        {
            Compilation = compilation,
            CompileUnitNamespace = @namespace,
            Lookups = Factory.ToModelLookup(),
            OutputFolder = outFolder,
        };
        Factory.AddResult(Parser.Parse(args));
    }

    public List<Constant> GetConstants()
    {
        return Factory.ConstFactory.GetValueCollection().ToList();
    }


    public Task WriteToDiskAsync()
    {
        var args = new WriterArgs()
        {
            Lookups = Factory.ToModelLookup(),
        };
        Writer.Write(args);

        HashSet<BaseCSharpOutFile> outFiles = new();

        foreach (var item in Factory.ConstFactory.GetValueCollection())
        {
            outFiles.Add(item.Output.OutputFile);
        }

        return Task.WhenAll(outFiles.Select(x => x.WriteToDiskAsync()).ToArray());
    }



}