using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel.BuiltinConstants
{
    public class IntMax : BaseBuiltInCsConstant
    {
        private static readonly InstanceId _id = new();

        private IntMax()
        {
        }

        public static IntMax Instance { get; } = new();

        public override string Name => "MaxValue";
        public override InstanceId Id => _id;
        public override string GetFullName() => "int.MaxValue";
        public override bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType) =>
            builtinType is Int32MaxBuiltinConstant;

        public override bool HasConstantValue() => true;
    }
}