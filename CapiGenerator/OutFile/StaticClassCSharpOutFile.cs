namespace CapiGenerator.OutFile;

using System.Text;

public sealed class StaticClassCSharpOutFile :
    BaseCSharpOutFile
{
    private readonly Dictionary<string, string> _memberNames = new();

    internal StaticClassCSharpOutFile(string namespaceValue, string path) :
        base(namespaceValue, path)
    {

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

    internal override Task WriteToDiskAsync()
    {
        var builder = new StringBuilder();

        builder.AppendLine($"namespace {Namespace};");
        builder.AppendLine();

        builder.AppendLine($"public partial static class {ClassName}");
        builder.Append("{");

        foreach (var member in _memberNames)
        {
            builder.AppendLine();
            builder.Append("\t");
            builder.Append(member.Value.Replace("\n", "\n\t"));
        }
        builder.AppendLine();
        builder.Append("}");

        return File.WriteAllTextAsync(Path, builder.ToString());
    }
}