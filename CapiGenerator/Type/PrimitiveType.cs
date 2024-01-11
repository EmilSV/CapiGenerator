using CapiGenerator.Model;

namespace CapiGenerator.Type;

public sealed class PrimitiveType : ICType
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    private sealed class NameAttribute : System.Attribute
    {
        readonly string[] names;

        // This is a positional argument
        public NameAttribute(params string[] names)
        {
            this.names = names;
        }

        public ReadOnlySpan<string> Names
        {
            get { return names; }
        }
    }

    public enum Kind
    {
        [Name("int8_t")] Int8_t,
        [Name("int16_t")] Int16_t,
        [Name("int32_t")] Int32_t,
        [Name("int64_t")] Int64_t,
        [Name("uint8_t")] UInt8_t,
        [Name("uint16_t")] UInt16_t,
        [Name("uint32_t")] UInt32_t,
        [Name("uint64_t")] UInt64_t,

        [Name("intptr_t")] Intptr_t,
        [Name("uintptr_t")] UIntptr_t,

        [Name("size_t")] Size_t,
        [Name("ssize_t")] SSize_t,

        [Name("ptrdiff_t")] Ptrdiff_t,

        [Name("char")] Char,
        [Name("signed char")] SignedChar,
        [Name("unsigned char")] UnsignedChar,

        [Name("short")] Short,
        [Name("short int")] ShortInt,
        [Name("signed short")] SignedShort,
        [Name("signed short int")] SignedShortInt,

        [Name("unsigned short")] UnsignedShort,
        [Name("unsigned short int")] UnsignedShortInt,

        [Name("int")] Int,
        [Name("signed")] Signed,
        [Name("signed int")] SignedInt,

        [Name("unsigned")] Unsigned,
        [Name("unsigned int")] UnsignedInt,

        [Name("long")] Long,
        [Name("long int")] LongInt,
        [Name("signed long")] SignedLong,
        [Name("signed long int")] SignedLongInt,

        [Name("unsigned long")] UnsignedLong,
        [Name("unsigned long int")] UnsignedLongInt,
        [Name("long long")] LongLong,
        [Name("long long int")] LongLongInt,
        [Name("signed long long")] SignedLongLong,
        [Name("signed long long int")] SignedLongLongInt,

        [Name("unsigned long long")] UnsignedLongLong,
        [Name("unsigned long long int")] UnsignedLongLongInt,

        [Name("float")] Float,
        [Name("double")] Double,
        [Name("long double")] LongDouble,

        [Name("_Bool")] _Bool,
        [Name("bool")] Bool,

        Other
    }

    private readonly string[] _names;
    private readonly Kind _kind;

    public string Name => _names.Length > 0 ?
        _names[0] :
        throw new InvalidOperationException();

    public ReadOnlySpan<string> Names => _names;
    public Kind KindValue => _kind;

    private PrimitiveType(Kind kind, ReadOnlySpan<string> names)
    {
        _kind = kind;
        _names = names.ToArray();
    }

    private static PrimitiveType[]? _allTypes = null;
    private static Dictionary<Kind, PrimitiveType>? _typesByKind = null;

    public static PrimitiveType GetByKind(Kind kind)
    {
        if (_typesByKind == null)
        {
            _typesByKind = [];
            foreach (var type in GetAllTypes())
            {
                _typesByKind.Add(type.KindValue, type);
            }
        }

        return _typesByKind[kind];
    }

    public static ReadOnlySpan<PrimitiveType> GetAllTypes() => _allTypes ??= typeof(Kind)
        .GetFields()
        .Where(field => field.GetCustomAttributes(typeof(NameAttribute), false).Length > 0)
        .Select(field =>
        {
            var names = ((NameAttribute)field.GetCustomAttributes(typeof(NameAttribute), false)[0]).Names;
            var kind = (Kind)field.GetRawConstantValue()!;
            return new PrimitiveType(kind, names);
        }).ToArray();
}