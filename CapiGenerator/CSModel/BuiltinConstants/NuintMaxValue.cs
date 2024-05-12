using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class NuintMaxValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private NuintMaxValue()
        {
        }

        public static NuintMaxValue Instance { get; } = new();

        public override string Name => "MaxValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "nuint.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is SizeMaxBuiltinConstant;

        public override bool HasConstantValue() => false;
    }
}