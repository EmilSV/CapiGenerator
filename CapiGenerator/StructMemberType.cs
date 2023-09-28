using CapiGenerator.Model;

namespace CapiGenerator;

public readonly struct StructMemberType
{
    private readonly object type;

    private StructMemberType(object type)
    {
        this.type = type;
    }

    public static implicit operator StructMemberType(Struct type) => new(type);
    public static implicit operator StructMemberType(Type type) => new(type);

    public bool TryGetCStruct(out Struct? cStruct)
    {
        if (type is Struct s)
        {
            cStruct = s;
            return true;
        }

        cStruct = null;
        return false;
    }

    public bool TryGetType(out Type? type)
    {
        if (this.type is Type t)
        {
            type = t;
            return true;
        }

        type = null;
        return false;
    }
}