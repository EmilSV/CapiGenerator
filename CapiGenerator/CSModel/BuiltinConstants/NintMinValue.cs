using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class NintMinValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private NintMinValue()
        {
        }

        public static NintMinValue Instance { get; } = new();

        public override string Name => "MinValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "nint.MinValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is IntPtrMinBuiltinConstant;

        public override bool HasConstantValue() => false;
    }
}