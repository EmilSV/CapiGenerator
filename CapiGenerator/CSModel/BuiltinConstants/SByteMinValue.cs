using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class SByteMinValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private SByteMinValue()
        {
        }

        public static SByteMinValue Instance { get; } = new();

        public override string Name => "MinValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "sbyte.MinValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is Int8MinBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}