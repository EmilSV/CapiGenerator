using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class SByteMaxValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private SByteMaxValue()
        {
        }

        public static SByteMaxValue Instance { get; } = new();

        public override string Name => "MaxValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "sbyte.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is Int8MaxBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}