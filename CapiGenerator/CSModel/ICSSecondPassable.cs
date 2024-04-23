using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public interface ICSSecondPassable
{
    void OnSecondPass(CSTranslationUnit compilationUnit);
}