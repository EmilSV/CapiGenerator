using System.Reflection;

namespace CapiGenerator.CSModel;

public class CSFixedInlineArrayType(uint size) : BaseCSTypeModifier
{
    public readonly uint Size = size;
    public override string GetTypePostFixString()
    {
        throw new Exception("FixedInlineArrayType do not support begin used as type postfix");
    }
}
