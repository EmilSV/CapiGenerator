namespace CapiGenerator.OutFile;

using PathIO = Path;

public abstract class BaseCSharpOutFile
{
    public readonly string Namespace;
    public readonly string Path;
    public readonly string ClassName;

    internal BaseCSharpOutFile(string namespaceValue, string path)
    {
        Path = path;
        Namespace = namespaceValue;
        ClassName = PathIO.GetFileNameWithoutExtension(Path);
    }

    internal abstract Task WriteToDiskAsync();
}