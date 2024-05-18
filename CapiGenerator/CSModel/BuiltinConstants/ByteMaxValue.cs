using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class ByteMaxValue : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private ByteMaxValue()
        {
        }

        public static ByteMaxValue Instance { get; } = new();

        public override string Name => "MaxValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "byte.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is UInt8MaxBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}