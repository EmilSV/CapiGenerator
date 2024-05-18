using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class ULongMaxValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private ULongMaxValue()
        {
        }

        public static ULongMaxValue Instance { get; } = new();

        public override string Name => "MaxValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "ulong.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is UInt64MaxBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}