using CapiGenerator.Model;

namespace CapiGenerator.ConstantToken;

public abstract class BaseConstantToken
{
    public abstract string? GetOutValue();
    public virtual void OnSecondPass(IReadOnlyDictionary<string, Constant> constants) { }
}
