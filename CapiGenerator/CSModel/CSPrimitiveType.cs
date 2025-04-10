using System.Diagnostics.CodeAnalysis;
using CapiGenerator.CModel;
using CapiGenerator.UtilTypes;

namespace CapiGenerator.CSModel;

public sealed class CSPrimitiveType : ICSType
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
    public InstanceId Id { get; } = new();

    public string? Namespace => null;

    public string Name { get; }

    public bool IsAnonymous => false;

    private CSPrimitiveType(Kind kind, string name)
    {
        KindValue = kind;
        Name = name;
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
        CConstantType.Int => Get(Kind.Int),
        CConstantType.UnsignedInt => Get(Kind.UInt),
        CConstantType.LongLong => Get(Kind.Long),
        CConstantType.UnsignedLongLong => Get(Kind.ULong),
        CConstantType.String => Get(Kind.String),
        CConstantType.Char => Get(Kind.Byte),
        CConstantType.Float => Get(Kind.Float),
        CConstantType.Double => Get(Kind.Double),
        CConstantType.Short => Get(Kind.Short),
        CConstantType.Long => Get(Kind.Long),
        CConstantType.UnsignedShort => Get(Kind.UShort),
        CConstantType.UnsignedLong => Get(Kind.ULong),
        _ => throw new ArgumentOutOfRangeException(nameof(constType))
    };

    public bool TryGetName([NotNullWhen(true)] out string? name)
    {
        name = Name;
        return true;
    }

    public static class Instances
    {
        public static readonly CSPrimitiveType Byte = Get(Kind.Byte);
        public static readonly CSPrimitiveType SByte = Get(Kind.SByte);
        public static readonly CSPrimitiveType Short = Get(Kind.Short);
        public static readonly CSPrimitiveType UShort = Get(Kind.UShort);
        public static readonly CSPrimitiveType Int = Get(Kind.Int);
        public static readonly CSPrimitiveType UInt = Get(Kind.UInt);
        public static readonly CSPrimitiveType Long = Get(Kind.Long);
        public static readonly CSPrimitiveType ULong = Get(Kind.ULong);
        public static readonly CSPrimitiveType Float = Get(Kind.Float);
        public static readonly CSPrimitiveType Double = Get(Kind.Double);
        public static readonly CSPrimitiveType NInt = Get(Kind.NInt);
        public static readonly CSPrimitiveType NUInt = Get(Kind.NUInt);
        public static readonly CSPrimitiveType Decimal = Get(Kind.Decimal);
        public static readonly CSPrimitiveType Bool = Get(Kind.Bool);
        public static readonly CSPrimitiveType Char = Get(Kind.Char);
        public static readonly CSPrimitiveType String = Get(Kind.String);
        public static readonly CSPrimitiveType Object = Get(Kind.Object);
        public static readonly CSPrimitiveType Void = Get(Kind.Void);
    }
}