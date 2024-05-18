using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class LongMaxValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private LongMaxValue()
        {
        }

        public static LongMaxValue Instance { get; } = new();

        public override string Name => "MaxValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "long.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is Int64MaxBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}