using System;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSFunctionTranslator : BaseTranslator
{
    public override void FirstPass(CSTranslationUnit translationUnit, ReadOnlySpan<CCompilationUnit> compilationUnits, BaseTranslatorOutputChannel outputChannel)
    {
        throw new NotImplementedException();
    }
    
}
