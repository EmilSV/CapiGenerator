using CapiGenerator.CModel;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSPrimitiveType : CSBaseType
{
    public enum Kind
    {
        Byte,
        SByte,
        Short,
        UShort,
        Int,
        UInt,
        Long,
        ULong,

        Float,
        Double,

        NInt,
        NUInt,

        Decimal,

        Bool,

        Char,
        String,

        Object,

        Void,
    }

    private static string GetName(Kind kind) => kind switch
    {
        Kind.Byte => "byte",
        Kind.SByte => "sbyte",
        Kind.Short => "short",
        Kind.UShort => "ushort",
        Kind.Int => "int",
        Kind.UInt => "uint",
        Kind.Long => "long",
        Kind.ULong => "ulong",
        Kind.Float => "float",
        Kind.Double => "double",
        Kind.NInt => "nint",
        Kind.NUInt => "nuint",
        Kind.Decimal => "decimal",
        Kind.Bool => "bool",
        Kind.Char => "char",
        Kind.String => "string",
        Kind.Object => "object",
        Kind.Void => "void",
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
    };

    private static Dictionary<Kind, CSPrimitiveType>? _allTypes;

    public readonly Kind KindValue;
    public override ComputedValueOrValue<string> FullName => Name;

    private CSPrimitiveType(Kind kind, string name) : base(name)
    {
        KindValue = kind;
        Name = HistoricValue<string>.NewReadOnly(name);
    }

    public static CSPrimitiveType Get(Kind kind)
    {
        if (_allTypes == null)
        {
            _allTypes = [];
            foreach (Kind value in Enum.GetValues<Kind>())
            {
                _allTypes[value] = new CSPrimitiveType(value, GetName(value));
            }
        }

        return _allTypes[kind];
    }

    public static CSPrimitiveType FromCConstType(CConstantType constType) => constType switch
    {
        CConstantType.Int => Get(Kind.Long),
        CConstantType.String => Get(Kind.String),
        CConstantType.Char => Get(Kind.Byte),
        CConstantType.Float => Get(Kind.Double),
        _ => throw new ArgumentOutOfRangeException(nameof(constType))
    };
}