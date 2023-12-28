using CapiGenerator.Parser;

namespace CapiGenerator.Model;


public class CFunction : INeedSecondPass
{
    public required string Name { get; init; }
    public required IReadOnlyList<CParameter> Parameters { get; init; }

    public void OnSecondPass(CCompilationUnit compilationUnit)
    {
        foreach (var parameter in Parameters)
        {
            parameter.OnSecondPass(compilationUnit);
        }
    }
}