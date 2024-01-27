using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSStructTranslator : BaseTranslator
{
    public override void Translator(
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var structItem in compilationUnit.GetStructEnumerable())
            {
                       
            }
        }
    }
}