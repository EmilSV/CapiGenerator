using CppAst;

namespace CapiGenerator.CModel.Type;

public sealed class CPrimitiveType : ICType
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

        [Name("short", "short int", "signed short", "signed short int")] Short,
        [Name("unsigned short", "unsigned short int")] UnsignedShortInt,

        [Name("signed", "int", "signed int")] Int,

        [Name("unsigned", "unsigned int")] UnsignedInt,

        [Name("long", "long int", "signed long", "signed long int")] Long,

        [Name("unsigned long", "unsigned long int")] UnsignedLong,

        [Name("long long", "long long int", "signed long long", "signed long long int")] LongLong,

        [Name("unsigned long long", "unsigned long long int")] UnsignedLongLong,

        [Name("float")] Float,
        [Name("double")] Double,
        [Name("long double")] LongDouble,

        [Name("_Bool")] _Bool,
        [Name("bool")] Bool,
        [Name("CString")] CString,
        [Name("void")] Void,

        Other
    }

    private readonly string[] _names;
    private readonly Kind _kind;

    public string Name => _names.Length > 0 ?
        _names[0] :
        throw new InvalidOperationException();

    public ReadOnlySpan<string> Names => _names;
    public Kind KindValue => _kind;

    private CPrimitiveType(Kind kind, ReadOnlySpan<string> names)
    {
        _kind = kind;
        _names = names.ToArray();
    }

    private static CPrimitiveType[]? _allTypes = null;
    private static Dictionary<Kind, CPrimitiveType>? _typesByKind = null;

    public static CPrimitiveType GetByKind(Kind kind)
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

    public static ReadOnlySpan<CPrimitiveType> GetAllTypes() => _allTypes ??= typeof(Kind)
        .GetFields()
        .Where(field => field.GetCustomAttributes(typeof(NameAttribute), false).Length > 0)
        .Select(field =>
        {
            var names = ((NameAttribute)field.GetCustomAttributes(typeof(NameAttribute), false)[0]).Names;
            var kind = (Kind)field.GetRawConstantValue()!;
            return new CPrimitiveType(kind, names);
        }).ToArray();

    public static CPrimitiveType FromCppPrimitiveType(CppPrimitiveType cppType) => cppType.Kind switch
    {
        CppPrimitiveKind.Void => GetByKind(Kind.Void),
        CppPrimitiveKind.Bool => GetByKind(Kind.Bool),
        CppPrimitiveKind.Char => GetByKind(Kind.Char),
        CppPrimitiveKind.Short => GetByKind(Kind.Short),
        CppPrimitiveKind.Int => GetByKind(Kind.Int),
        CppPrimitiveKind.LongLong => GetByKind(Kind.LongLong),
        CppPrimitiveKind.Float => GetByKind(Kind.Float),
        CppPrimitiveKind.Double => GetByKind(Kind.Double),
        CppPrimitiveKind.LongDouble => GetByKind(Kind.LongDouble),
        CppPrimitiveKind.UnsignedChar => GetByKind(Kind.UnsignedChar),
        CppPrimitiveKind.UnsignedShort => GetByKind(Kind.UnsignedShortInt),
        CppPrimitiveKind.UnsignedInt => GetByKind(Kind.UnsignedInt),
        CppPrimitiveKind.UnsignedLongLong => GetByKind(Kind.UnsignedLongLong),
        _ => throw new NotImplementedException(),
    };

    public static CPrimitiveType FromCConstType(CConstantType constType) => constType switch
    {
        CConstantType.Int => GetByKind(Kind.LongLong),
        CConstantType.String => GetByKind(Kind.CString),
        CConstantType.Char => GetByKind(Kind.Char),
        CConstantType.Float => GetByKind(Kind.Double),
        _ => throw new ArgumentOutOfRangeException(nameof(constType))
    };

    public static class Instances
    {
        public static readonly CPrimitiveType Int8_t = GetByKind(Kind.Int8_t);
        public static readonly CPrimitiveType Int16_t = GetByKind(Kind.Int16_t);
        public static readonly CPrimitiveType Int32_t = GetByKind(Kind.Int32_t);
        public static readonly CPrimitiveType Int64_t = GetByKind(Kind.Int64_t);
        public static readonly CPrimitiveType UInt8_t = GetByKind(Kind.UInt8_t);
        public static readonly CPrimitiveType UInt16_t = GetByKind(Kind.UInt16_t);
        public static readonly CPrimitiveType UInt32_t = GetByKind(Kind.UInt32_t);
        public static readonly CPrimitiveType UInt64_t = GetByKind(Kind.UInt64_t);
        public static readonly CPrimitiveType Intptr_t = GetByKind(Kind.Intptr_t);
        public static readonly CPrimitiveType UIntptr_t = GetByKind(Kind.UIntptr_t);
        public static readonly CPrimitiveType Size_t = GetByKind(Kind.Size_t);
        public static readonly CPrimitiveType SSize_t = GetByKind(Kind.SSize_t);
        public static readonly CPrimitiveType Ptrdiff_t = GetByKind(Kind.Ptrdiff_t);
        public static readonly CPrimitiveType Char = GetByKind(Kind.Char);
        public static readonly CPrimitiveType SignedChar = GetByKind(Kind.SignedChar);
        public static readonly CPrimitiveType UnsignedChar = GetByKind(Kind.UnsignedChar);
        public static readonly CPrimitiveType Short = GetByKind(Kind.Short);
        public static readonly CPrimitiveType UnsignedShortInt = GetByKind(Kind.UnsignedShortInt);
        public static readonly CPrimitiveType Int = GetByKind(Kind.Int);
        public static readonly CPrimitiveType UnsignedInt = GetByKind(Kind.UnsignedInt);
        public static readonly CPrimitiveType Long = GetByKind(Kind.Long);
        public static readonly CPrimitiveType UnsignedLong = GetByKind(Kind.UnsignedLong);
        public static readonly CPrimitiveType LongLong = GetByKind(Kind.LongLong);
        public static readonly CPrimitiveType UnsignedLongLong = GetByKind(Kind.UnsignedLongLong);
        public static readonly CPrimitiveType Float = GetByKind(Kind.Float);
        public static readonly CPrimitiveType Double = GetByKind(Kind.Double);
        public static readonly CPrimitiveType LongDouble = GetByKind(Kind.LongDouble);
        public static readonly CPrimitiveType _Bool = GetByKind(Kind._Bool);
        public static readonly CPrimitiveType Bool = GetByKind(Kind.Bool);
        public static readonly CPrimitiveType CString = GetByKind(Kind.CString);
        public static readonly CPrimitiveType Void = GetByKind(Kind.Void);
    }

}