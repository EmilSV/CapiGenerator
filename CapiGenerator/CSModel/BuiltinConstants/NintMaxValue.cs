using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class NintMaxValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private NintMaxValue()
        {
        }

        public static NintMaxValue Instance { get; } = new();

        public override string Name => "MaxValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "nint.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is IntPtrMaxBuiltinConstant;

        public override bool HasConstantValue() => false;
    }
}