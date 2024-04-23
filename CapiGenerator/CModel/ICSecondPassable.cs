using CapiGenerator.Parser;

namespace CapiGenerator.CModel;

public interface ICSecondPassable
{
    void OnSecondPass(CCompilationUnit compilationUnit);
}