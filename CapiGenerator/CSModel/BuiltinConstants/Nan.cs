using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants;

public class Nan : BaseBuiltInCsConstant
{
    private static readonly InstanceId _id = new();

    private Nan()
    {
    }

    public static Nan Instance { get; } = new();

    public override string Name => "NaN";
    public override InstanceId Id => _id;
    public override string GetFullName() => "float.NaN";
    public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
        builtinType is NanBuiltinConstant;

    public override bool HasConstantValue() => true;
}