namespace CapiGenerator.CSModel.Type;

public sealed class CSPrimitiveType
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
        [Name("byte")] Byte,
        [Name("sbyte")] SByte,
        [Name("short")] Short,
        [Name("ushort")] UShort,
        [Name("int")] Int,
        [Name("uint")] UInt,
        [Name("long")] Long,
        [Name("ulong")] ULong,
        
        [Name("float")] Float,
        [Name("double")] Double,
        
        [Name("decimal")] Decimal,

        [Name("bool")] Bool,
        
        [Name("char")] Char,
        [Name("string")] String,
        
        [Name("object")] Object,
        
        Other
    }
    
    private readonly string[] _names;
    private readonly Kind _kind;

    public string Name => _names.Length > 0 ?
        _names[0] :
        throw new InvalidOperationException();

    public ReadOnlySpan<string> Names => _names;
    public Kind KindValue => _kind;

    private CSPrimitiveType(Kind kind, ReadOnlySpan<string> names)
    {
        _kind = kind;
        _names = names.ToArray();
    }

    private static CSPrimitiveType[]? _allTypes = null;
    private static Dictionary<Kind, CSPrimitiveType>? _typesByKind = null;

    public static CSPrimitiveType GetByKind(Kind kind)
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

    public static ReadOnlySpan<CSPrimitiveType> GetAllTypes() => _allTypes ??= typeof(Kind)
        .GetFields()
        .Where(field => field.GetCustomAttributes(typeof(NameAttribute), false).Length > 0)
        .Select(field =>
        {
            var names = ((NameAttribute)field.GetCustomAttributes(typeof(NameAttribute), false)[0]).Names;
            var kind = (Kind)field.GetRawConstantValue()!;
            return new CSPrimitiveType(kind, names);
        }).ToArray();
}