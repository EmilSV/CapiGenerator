using CppAst;

namespace CapiGenerator.CModel.BuiltinTypedefs;

public class Int16TBuiltinTypedef : BaseBuiltinTypedef
{
    public static Int16TBuiltinTypedef Instance { get; } = new Int16TBuiltinTypedef();

    public override string Name => "int16_t";

    private Int16TBuiltinTypedef()
    {
    }
    public override bool TypedefIsBuiltin(CppTypedef typedef)
    {
        return typedef.Name == "int16_t" && typedef.ElementType is CppPrimitiveType { Kind: CppPrimitiveKind.Short };
    }
}