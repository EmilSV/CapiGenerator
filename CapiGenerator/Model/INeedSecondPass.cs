using CapiGenerator.Parser;

namespace CapiGenerator.Model;

public interface INeedSecondPass
{
    void OnSecondPass(CCompilationUnit compilationUnit);
}
