using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class ShortMinValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private ShortMinValue()
        {
        }

        public static ShortMinValue Instance { get; } = new();

        public override string Name => "MinValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "short.MinValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is Int16MinBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}