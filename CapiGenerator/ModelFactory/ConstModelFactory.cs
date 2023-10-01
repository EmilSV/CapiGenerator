using CapiGenerator.Model;

namespace CapiGenerator.ModelFactory;

public sealed class ConstModelFactory : BaseModelRefLookup<Constant>
{
    public Constant CreateConstant(Constant.ConstantInput input, Constant.ConstantOutput output)
    {
        var constant = new Constant(GetRef(input.Name, input.CompileUnitNamespace), this, input, output);
        _guidLookup.Add(constant.ModelRef.Guid, constant);
        return constant;
    }
}