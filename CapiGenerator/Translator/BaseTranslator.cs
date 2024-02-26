using CapiGenerator.CSModel;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public abstract class BaseTranslator
{
    public abstract void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel
    );
    public virtual void SecondPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorInputChannel inputChannel
    ) {}
}