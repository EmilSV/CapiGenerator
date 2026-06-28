using CapiGenerator.CSModel;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public abstract class BaseTranslator
{
    protected TranslatorCollection Translators
    {
        get => field ?? throw new InvalidOperationException("Translators not initialized");
        private set;
    }

    public virtual void Init(TranslatorCollection translatorCollection)
    {
        Translators = translatorCollection;
    }

    public abstract void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel
    );
    public virtual void SecondPass(
        CSTranslationUnit translationUnit,
        BaseTranslatorInputChannel inputChannel
    )
    { }
}
