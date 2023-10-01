using CapiGenerator.Model;

namespace CapiGenerator.ConstantToken;

public abstract record class BaseConstantToken
{
    public abstract string GetOutValue(BaseModelRefLookup<Constant> constLookup);
}
