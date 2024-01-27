using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public abstract class BaseTranslator
{
    public abstract void Translator(
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel
    );
}