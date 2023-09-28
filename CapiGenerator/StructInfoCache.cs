using System.Diagnostics;
using CppAst;

public static class StructInfoCache
{
    public class Item
    {
        public required string Name;
        public bool isUnsafe;
    }

    private static readonly Dictionary<string, Item> StructsLookup = new();

    public static Item? Get(CppClass value)
    {
        if (value.Name == "WGPURequestAdapterOptions")
        {
            Debugger.Break();
        }

        if (value.ClassKind != CppClassKind.Struct || !value.IsDefinition)
        {
            return null;
        }

        {
            if (StructsLookup.TryGetValue(value.Name, out var item))
            {
                return item;
            }
        }

        CppClass[] classFields = value.Fields.Select(
            f => f.Type as CppClass
        ).Where(f => f != null).ToArray()!;

        bool isUnsafe = false;

        foreach (var field in value.Fields)
        {
            if (IsTypeUnsafe(field.Type))
            {
                isUnsafe = true;
                break;
            }
        }

        if (classFields.Length > 0)
        {
            foreach (var field in classFields)
            {
                var item = Get(field);
                if (item != null && item.isUnsafe && !item.Name.StartsWith("ChainedStruct"))
                {
                    isUnsafe = true;
                    break;
                }
            }
        }

        const string PREFIX = "WGPU";
        string name = value.Name;
        if (name.StartsWith(PREFIX))
        {
            name = name[PREFIX.Length..];
        }

        if (name.StartsWith("ChainedStruct"))
        {
            isUnsafe = false;
        }

        if (isUnsafe)
        {
            name = $"{name}.Unmanaged";
        }

        Item newItem = new() { Name = name, isUnsafe = isUnsafe };
        StructsLookup.Add(value.Name, newItem);

        return newItem;
    }

    private static bool IsTypeUnsafe(CppType type) => type switch
    {
        CppPointerType _ or CppArrayType _ => true,
        _ => false,
    } && !type.FullName.Contains("WGPUChainedStruct");
}