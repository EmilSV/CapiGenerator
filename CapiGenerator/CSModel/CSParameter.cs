using CapiGenerator.CModel;
using CapiGenerator.Translator;

namespace CapiGenerator.CSModel;

public sealed class CSParameter(
    string name, CSTypeInstance type, CSDefaultValue defaultValue = default
) : BaseCSAstItem
{
    public string Name => name;
    public CSTypeInstance Type => type;
    public CSDefaultValue? DefaultValue => defaultValue;

    public override void OnSecondPass(CSTranslationUnit unit)
    {
        type.OnSecondPass(unit);
    }

    public static CSParameter FromCParameter(CParameter parameter) => new(
        name: parameter.Name,
        type: CSTypeInstance.CreateFromCTypeInstance(parameter.GetParameterType()),
        defaultValue: CSDefaultValue.NullValue
    );
}