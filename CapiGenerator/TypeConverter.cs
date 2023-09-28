using System.Diagnostics;
using System.Text;
using CppAst;

namespace CapiGenerator;

static class TypeConverter
{
    public static string ConvertToCSharpType(CppType type, bool isPointer = false)
    {
        if (type is CppPrimitiveType primitiveType)
        {
            return GetCsTypeName(primitiveType, isPointer);
        }

        if (type is CppQualifiedType qualifiedType)
        {
            return GetCsTypeName(qualifiedType.ElementType, isPointer);
        }

        if (type is CppEnum enumType)
        {
            return GetEnumName(enumType, isPointer);
        }

        if (AsFunctionType(type) is CppFunctionType functionType)
        {
            var functionTypeName = GetCsTypeName(functionType);
            return functionTypeName;
        }

        if (type is CppTypedef typedef)
        {
            if (IsHandelTypeDef(typedef))
            {
                return GetHandelName(typedef);
            }

            if (IsFlagTypeDef(typedef))
            {
                return GetFlagTypedefName(typedef, isPointer);
            }

            var typeDefCsName = GetCsCleanName(typedef.Name);
            if (isPointer)
                return typeDefCsName + "*";

            return typeDefCsName;
        }

        if (type is CppClass @class)
        {
            if (IsStruct(@class))
            {
                return GetStructName(@class, isPointer);
            }

            var className = GetCsCleanName(@class.Name);
            if (isPointer)
                return className + "*";

            return className;
        }

        if (type is CppPointerType pointerType)
        {
            return GetCsTypeName(pointerType);
        }

        if (type is CppArrayType arrayType)
        {
            return GetCsTypeName(arrayType.ElementType, isPointer);
        }

        return string.Empty;
    }

    private static string GetCsTypeName(CppPointerType pointerType)
    {
        if (pointerType.ElementType is CppQualifiedType qualifiedType)
        {
            if (qualifiedType.ElementType is CppPrimitiveType primitiveType)
            {
                return GetCsTypeName(primitiveType, true);
            }
            else if (qualifiedType.ElementType is CppClass @classType)
            {
                if (IsStruct(@classType))
                {
                    return GetStructName(@classType, true);
                }
                return GetCsTypeName(@classType, true);
            }
            else if (qualifiedType.ElementType is CppPointerType subPointerType)
            {
                return GetCsTypeName(subPointerType, true) + "*";
            }
            else if (qualifiedType.ElementType is CppTypedef typedef)
            {
                if (IsHandelTypeDef(typedef))
                {
                    return GetHandelName(typedef) + "*";
                }

                if (IsFlagTypeDef(typedef))
                {
                    return GetFlagTypedefName(typedef, true);
                }

                return GetCsTypeName(typedef, true);
            }
            else if (qualifiedType.ElementType is CppEnum @enum)
            {
                return GetEnumName(@enum, true);
            }

            return GetCsTypeName(qualifiedType.ElementType, true);
        }

        return GetCsTypeName(pointerType.ElementType, true);
    }

    private static string GetCsTypeName(CppPrimitiveType primitiveType, bool isPointer)
    {
        string result = string.Empty;

        switch (primitiveType.Kind)
        {
            case CppPrimitiveKind.Void:
                result = "void";
                break;
            case CppPrimitiveKind.Bool:
                result = "bool";
                break;
            case CppPrimitiveKind.Char:
                result = "byte";
                break;
            case CppPrimitiveKind.Short:
                result = "short";
                break;
            case CppPrimitiveKind.Int:
                result = "int";
                break;
            case CppPrimitiveKind.UnsignedShort:
                result = "ushort";
                break;
            case CppPrimitiveKind.UnsignedInt:
                result = "uint";
                break;
            case CppPrimitiveKind.Float:
                result = "float";
                break;
            case CppPrimitiveKind.Double:
                result = "double";
                break;
            case CppPrimitiveKind.UnsignedChar:
            case CppPrimitiveKind.LongLong:
            case CppPrimitiveKind.UnsignedLongLong:
            case CppPrimitiveKind.LongDouble:
            case CppPrimitiveKind.WChar:
            default:
                break;
        }

        if (isPointer)
        {
            result += "*";
        }

        return result;
    }

    private static string GetCsTypeName(CppFunctionType functionType)
    {
        StringBuilder builder = new();
        builder.Append("delegate* unmanaged[Cdecl]<");
        string[] parameters = functionType.Parameters.Select(x => ConvertToCSharpType(x.Type)).ToArray();
        builder.Append(string.Join(", ", parameters));
        builder.Append(", ");
        builder.Append(ConvertToCSharpType(functionType.ReturnType));
        builder.Append(">");
        return builder.ToString();
    }

    private static string GetCsTypeName(CppType type, bool isPointer = false)
    {
        if (type is CppPrimitiveType primitiveType)
        {
            return GetCsTypeName(primitiveType, isPointer);
        }

        if (type is CppQualifiedType qualifiedType)
        {
            return GetCsTypeName(qualifiedType.ElementType, isPointer);
        }

        if (type is CppEnum enumType)
        {
            return GetEnumName(enumType, isPointer);
        }

        if (AsFunctionType(type) is CppFunctionType functionType)
        {
            var functionTypeName = GetCsTypeName(functionType);
            return functionTypeName;
        }

        if (type is CppTypedef typedef)
        {
            if (IsHandelTypeDef(typedef))
            {
                return GetHandelName(typedef);
            }

            if (IsFlagTypeDef(typedef))
            {
                return GetFlagTypedefName(typedef, isPointer);
            }

            var typeDefCsName = GetCsCleanName(typedef.Name);
            if (isPointer)
                return typeDefCsName + "*";

            return typeDefCsName;
        }

        if (type is CppClass @class)
        {
            if (IsStruct(@class))
            {
                return GetStructName(@class, isPointer);
            }

            var className = GetCsCleanName(@class.Name);
            if (isPointer)
                return className + "*";

            return className;
        }

        if (type is CppPointerType pointerType)
        {
            return GetCsTypeName(pointerType);
        }

        if (type is CppArrayType arrayType)
        {
            return GetCsTypeName(arrayType.ElementType, isPointer);
        }

        return string.Empty;
    }

    public static bool IsHandelTypeDef(CppTypedef typedef)
    {
        string typeName = typedef.Name;

        if (typeName.StartsWith("WGPUProc") || typeName.EndsWith("Callback"))
        {
            return false;
        }

        if (typedef.ElementType is not CppPointerType)
        {
            return false;
        }

        return true;
    }

    public static CppEnum? AsEnum(CppTypedef typedef)
    {
        if (typedef.ElementType is CppEnum cppEnum)
        {
            return cppEnum;
        }

        if (typedef.ElementType is CppTypedef subTypedef)
        {
            return AsEnum(subTypedef);
        }

        return null;
    }

    static bool IsFlagTypeDef(CppTypedef typedef)
    {
        static bool IsInnerTypeWGPUFlags(CppTypedef typedef)
        {
            if (typedef.ElementType is CppTypedef subTypedef)
            {
                if (subTypedef.Name == "WGPUFlags")
                {
                    return true;
                }
                else
                {
                    return IsInnerTypeWGPUFlags(subTypedef);
                }
            }

            return false;
        }

        if (!IsInnerTypeWGPUFlags(typedef))
        {
            return false;
        }

        string typeName = typedef.Name;

        if (typeName.StartsWith("WGPUProc"))
        {
            return false;
        }

        if (typeName.EndsWith("Flags"))
        {
            return true;
        }

        return false;
    }

    public static string GetCsCleanName(string name)
    {
        if (MapName(name) is string mappedName)
        {
            return mappedName;
        }

        if (name.StartsWith("PFN"))
        {
            return "IntPtr";
        }

        if (name.Contains("Flags"))
        {
            return name.Remove(name.Count() - 5);
        }
        return name;
    }

    public static string GetFlagTypedefName(CppTypedef typedef, bool isPointer = false)
    {
        const string PREFIX = "WGPU";
        string name = typedef.Name;
        if (name.StartsWith(PREFIX))
        {
            name = name[PREFIX.Length..];
        }

        if (name.Contains("Flags"))
        {
            name = name.Remove(name.Count() - 5);
        }

        if (isPointer)
        {
            return name + "*";
        }

        return name;
    }

    public static string GetEnumName(CppEnum cppEnum, bool isPointer = false)
    {
        const string PREFIX = "WGPU";
        string name = cppEnum.Name;
        if (name.StartsWith(PREFIX))
        {
            name = name[PREFIX.Length..];
        }

        if (isPointer)
        {
            return name + "*";
        }

        return name;
    }


    public static string GetHandelName(CppTypedef typedef)
    {
        const string PREFIX = "WGPU";
        string name = typedef.Name;
        if (name.StartsWith(PREFIX))
        {
            name = name[PREFIX.Length..];
        }
        return name + "Handle";
    }

    public static bool IsStruct(CppClass cppClass) =>
        cppClass.ClassKind == CppClassKind.Struct &&
        cppClass.IsDefinition;


    public static string GetStructName(CppClass cppClass, bool isPointer = false)
    {
        var item = StructInfoCache.Get(cppClass);
        string name = item?.Name ?? cppClass.Name;
        
        if (isPointer)
        {
            return name + "*";
        }

        return name;
    }

    static string? MapName(string value) => value switch
    {
        "uint8_t" => "byte",
        "uint16_t" => "ushort",
        "uint32_t" => "uint",
        "uint64_t" => "ulong",
        "int8_t" => "sbyte",
        "int32_t" => "int",
        "int16_t" => "short",
        "int64_t" => "long",
        "int64_t*" => "long*",
        "char" => "byte",
        "size_t" => "nuint",
        _ => null,
    };

    static CppFunctionType? AsFunctionType(CppType type)
    {
        if (type is CppFunctionType functionType)
        {
            return functionType;
        }

        if (type is CppPointerType pointerType)
        {
            return AsFunctionType(pointerType.ElementType);
        }

        if (type is CppTypedef typedef)
        {
            return AsFunctionType(typedef.ElementType);
        }

        return null;
    }
}