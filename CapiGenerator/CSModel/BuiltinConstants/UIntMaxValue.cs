using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class UIntMaxValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private UIntMaxValue()
        {
        }

        public static UIntMaxValue Instance { get; } = new();

        public override string Name => "MaxValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "uint.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is UInt32MaxBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}