namespace CapiGenerator;

using System.Text;
using PathIO = Path;

public sealed class CSharpOutFile
{
    private readonly Dictionary<string, string> _memberNames = new();
    public readonly string Path;
    public readonly ClassType ClassType;

    internal CSharpOutFile(string path, ClassType classType)
    {
        Path = path;
        ClassType = classType;
    }

    public bool IsNameUsed(string name)
    {
        return _memberNames.ContainsKey(name);
    }

    public void Add(string name, string code)
    {
        if (!_memberNames.TryAdd(name, code))
        {
            throw new ArgumentException($"Name {name} is already used in {Path}");
        }
    }


    internal Task WriteToDiskAsync()
    {
        var className = PathIO.GetFileNameWithoutExtension(Path);

        var builder = new StringBuilder();

        builder.AppendLine(ClassType switch
        {
            ClassType.Class => $"public class {className}",
            ClassType.Struct => $"public struct {className}",
            ClassType.Enum => $"public enum {className}",
            ClassType.StaticClass => $"public static class {className}",
            _ => throw new NotImplementedException(),
        });

        builder.AppendLine("{");

        foreach (var member in _memberNames)
        {
            builder.AppendLine(member.Value);
        }

        builder.AppendLine("}");

        return File.WriteAllTextAsync(Path, builder.ToString());
    }
}