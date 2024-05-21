using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public abstract class BaseBuiltInCsConstant : ICSFieldLike
{
    public abstract string Name { get; }
    public abstract InstanceId Id { get; }
    public uint ChangeCount => 0;
    public abstract string GetFullName();
    public abstract bool CConstantTranslateToBuiltin(BaseBuiltInCConstant builtinType);
    public abstract bool HasConstantValue();
}