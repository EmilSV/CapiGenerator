using CapiGenerator.Model;

namespace CapiGenerator.ConstantToken;

public abstract record class BaseConstantToken
{
    public abstract string GetValue(IReadOnlyDictionary<Guid, Constant> constants);
}
