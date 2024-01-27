using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;


public class CConstTranslator(
    string className,
    Func<CConstant, bool>? predicate,
    Func<CConstant, string>? customNameSelector = null
)
    : BaseTranslator
{

    private string nameSelector(CConstant value) => customNameSelector?.Invoke(value) ?? value.Name;
    private bool predicateSelector(CConstant value) => predicate?.Invoke(value) ?? true;


    public override void Translator(
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        List<CSField> fields = [];

        CSStaticClass newCSStaticClass = new(className,);   
    }
}