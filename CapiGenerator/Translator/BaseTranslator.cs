using CapiGenerator.CSModel;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public abstract class BaseTranslator
{
    public abstract void FirstPass(
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseCSTypeResolver typeResolver,
        BaseTranslatorOutputChannel outputChannel
    );
    public virtual void SecondPass(
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorInputChannel inputChannel
    ) {}
}