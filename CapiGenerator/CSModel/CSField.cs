using CapiGenerator.CModel;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSField(
    string name, CSTypeInstance type, CSDefaultValue defaultValue = default
) : BaseCSAstItem
{
    public string Name => name;
    public CSTypeInstance Type => type;
    public CSDefaultValue DefaultValue => defaultValue;

    public bool IsConst { get; init; }
    public bool IsStatic { get; init; }
    public bool IsReadOnly { get; init; }
    public CSAccessModifier AccessModifier { get; init; } = CSAccessModifier.Public;

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        type.OnSecondPass(unit);
    }
}