using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;


public class CConstTranslator(string className) : BaseTranslator
{
    protected virtual string NameSelector(CConstant value) => value.Name;
    protected virtual bool PredicateSelector(CConstant value) => true;

    public override void Translator(
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseCSTypeResolver typeResolver,
        BaseTranslatorOutputChannel outputChannel)
    {
        List<CSField> fields = [];
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var constItem in compilationUnit.GetConstantEnumerable())
            {
                if (PredicateSelector(constItem))
                {
                    fields.Add(TranslateConstant(constItem, typeResolver));
                }
            }
        }
        outputChannel.OnReceiveStaticClass(new CSStaticClass(className, fields.ToArray(), []));

    }

    private CSField TranslateConstant(CConstant constItem, BaseCSTypeResolver typeResolver)
    {
        CSResolveType fieldType = new(
            cType: CPrimitiveType.FromCConstType(constItem.GetConstantType()),
            resolvedType: CSPrimitiveType.FromCConstType(constItem.GetConstantType())
        );
        var newField = new CSField(NameSelector(constItem), fieldType, null, new() { Const = true });
        newField.EnrichingDataStore.Add(new CSTranslationCAstData(constItem));
        return newField;
    }
}