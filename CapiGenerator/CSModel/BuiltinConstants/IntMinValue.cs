using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class IntMinValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private IntMinValue()
        {
        }

        public static IntMinValue Instance { get; } = new();

        public override string Name => "MinValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "int.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is Int32MinBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}