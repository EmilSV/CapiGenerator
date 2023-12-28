using CapiGenerator.Parser;

namespace CapiGenerator.Model;

public class CStructModel(string name, ReadOnlySpan<CField> parameters) : INeedSecondPass
{
    public readonly string Name = name;
    public readonly IReadOnlyList<CField> Fields = parameters.ToArray();

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var field in Fields)
        {
            if (field is INeedSecondPass needSecondPass)
            {
                needSecondPass.OnSecondPass(compilationUnit);
            }
        }
    }
}