namespace CapiGenerator.Translator;


public sealed class TranslatorCollection
{
    private readonly List<BaseTranslator> _translators;

    internal TranslatorCollection(List<BaseTranslator> translators)
    {
        _translators = translators;
    }

    public T? GetTranslator<T>() where T : BaseTranslator
    {
        return _translators.OfType<T>().FirstOrDefault();
    }

    public IEnumerable<T> GetTranslators<T>() where T : BaseTranslator
    {
        return _translators.OfType<T>();
    }
}
