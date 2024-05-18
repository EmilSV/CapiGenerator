using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class UShortMaxValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private UShortMaxValue()
        {
        }

        public static UShortMaxValue Instance { get; } = new();

        public override string Name => "MaxValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "ushort.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is UInt16MaxBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}