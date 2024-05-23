using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class LongMinValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private LongMinValue()
        {
        }

        public static LongMinValue Instance { get; } = new();

        public override string Name => "MinValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "long.MinValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is Int64MinBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}