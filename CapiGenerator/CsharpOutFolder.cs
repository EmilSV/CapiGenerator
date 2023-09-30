namespace CapiGenerator;

using PathIO = Path;

public class CsharpOutFolder
{
    private readonly Dictionary<string, CSharpOutFile> _lookup = new();
    private readonly string Path;

    public CsharpOutFolder(string path)
    {
        Path = PathIO.GetFullPath(path);
    }

    public CSharpOutFile GetFile(string className, ClassType classType)
    {
        var fileName = $"{className}.cs";
        var filePath = PathIO.Combine(Path, fileName);
        if (_lookup.TryGetValue(filePath, out var outFile))
        {
            if (outFile.ClassType != classType)
            {
                throw new Exception($"File {filePath} is already used for {outFile.ClassType}.");
            }
            return outFile;
        }
        outFile = new CSharpOutFile(filePath, classType);
        _lookup.Add(filePath, outFile);

        return outFile;
    }
}