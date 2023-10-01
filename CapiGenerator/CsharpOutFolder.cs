namespace CapiGenerator;

using CapiGenerator.OutFile;
using PathIO = Path;

public class CsharpOutFolder
{
    private readonly Dictionary<string, BaseCSharpOutFile> _lookup = new();
    public readonly string Path;
    public readonly string Namespace;

    internal CsharpOutFolder(string path, string @namespace)
    {
        Path = path;
        Namespace = @namespace;
    }

    public T GetFile<T>(string className)
        where T : BaseCSharpOutFile
    {
        var fileName = $"{className}.cs";
        var filePath = PathIO.Combine(Path, fileName);
        if (_lookup.TryGetValue(filePath, out var outFile))
        {
            if (outFile is T cachedOutFile)
            {
                return cachedOutFile;
            }
            else
            {
                throw new Exception($"File {filePath} is already used for {outFile.ClassName}.");
            }
        }

        if(typeof(T) == typeof(EnumCSharpOutFile))
        {
            outFile = new EnumCSharpOutFile(Namespace, filePath);
        }
        else if (typeof(T) == typeof(StaticClassCSharpOutFile))
        {
            outFile = new StaticClassCSharpOutFile(Namespace, filePath);
        }
        else if (typeof(T) == typeof(StructCSharpOutFile))
        {
            outFile = new StructCSharpOutFile(Namespace, filePath);
        }
        else if (typeof(T) == typeof(ClassCSharpOutFile))
        {
            outFile = new ClassCSharpOutFile(Namespace, filePath);
        }
        else
        {
            throw new Exception($"Unknown type {typeof(T)}.");
        }

        _lookup.Add(filePath, outFile);

        return (T)outFile;
    }
}