using CapiGenerator.CModel;

namespace CapiGenerator.CSModel.ConstantToken;

public sealed class CSConstCastToken(CConstantType type) : BaseCSConstantToken
{
    public override string ToString() => $"({GetTypeName()})";

    private string GetTypeName() => type switch
    {
        CConstantType.Char or CConstantType.UInt8_t => "byte",
        CConstantType.Int8_t => "sbyte",
        CConstantType.Short or CConstantType.Int16_t => "short",
        CConstantType.UnsignedShort or CConstantType.UInt16_t => "ushort",
        CConstantType.Int or CConstantType.Int32_t => "int",
        CConstantType.UnsignedInt or CConstantType.UInt32_t => "uint",
        CConstantType.LongLong or CConstantType.Int64_t => "long",
        CConstantType.UnsignedLongLong or CConstantType.UInt64_t => "ulong",
        CConstantType.IntPtr_t => "nint",
        CConstantType.UIntPtr_t or CConstantType.Size_t => "nuint",
        CConstantType.Float => "float",
        CConstantType.Double => "double",
        CConstantType.Long => "CLong",
        CConstantType.UnsignedLong => "CULong",
        _ => throw new InvalidOperationException($"Unsupported constant cast type: {type}"),
    };
}
