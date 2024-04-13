using CapiGenerator.Translator;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public class CSStaticClass : CSBaseType
{
    public ChangeCountList<CSField> Fields { get; init; } = [];
    public ChangeCountList<CSMethod> Methods { get; init; } = [];

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        foreach (var field in Fields)
        {
            field.OnSecondPass(unit);
        }
        foreach (var method in Methods)
        {
            method.OnSecondPass(unit);
        }
    }
}

