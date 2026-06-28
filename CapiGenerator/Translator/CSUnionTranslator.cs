using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSUnionTranslator : CSStructTranslator
{
    public override void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var unionItem in compilationUnit.GetUnionEnumerable())
            {
                if (translationUnit.IsTypeTranslated(unionItem))
                {
                    continue;
                }

                outputChannel.OnReceiveStruct(TranslateUnionRecord(unionItem));
            }
        }
    }
}
