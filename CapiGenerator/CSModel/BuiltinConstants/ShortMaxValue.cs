using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class ShortMaxValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private ShortMaxValue()
        {
        }

        public static ShortMaxValue Instance { get; } = new();

        public override string Name => "MaxValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "short.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is Int16MaxBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}