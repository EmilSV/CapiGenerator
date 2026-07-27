using CapiGenerator.CSModel;

namespace CapiGenerator.Fixer;

public static class InvalidNameFixer
{
    private static readonly HashSet<string> _csKeywords = [
        "abstract","as","base","bool","break","byte",
        "case","catch","char","checked","class",
        "const","continue","decimal","default",
        "delegate","do","double","else","enum",
        "event","explicit","extern","false",
        "finally","fixed","float","for","foreach",
        "goto","if","implicit","in","int",
        "interface","internal","is","lock","long",
        "namespace","new","null","object","operator",
        "out","override","params","private",
        "protected","public","readonly","ref",
        "return","sbyte","sealed","short",
        "sizeof","stackalloc","static",
        "string","struct","switch","this",
        "throw","true","try","typeof",
        "uint","ulong","unchecked","unsafe",
        "ushort","using","virtual","void",
        "volatile","while"
    ];

    private static readonly HashSet<string> _contextualKeywords = [
        "add","allows","alias","and","ascending","args","async",
        "await","by","closed","descending","dynamic","equals",
        "extension","field","file","from","get","global",
        "group","init","into","join","let","managed","nameof",
        "nint","not","notnull","nuint","on","or","orderby",
        "partial","record","remove","required","safe","scoped",
        "select","set","unmanaged","value","var","when",
        "where","with","yield",
    ];

    public enum NameContext
    {
        Struct,
        Method,
        Field,
        Parameter,
        Local,
    }

    public static void FixInvalidNames(
        IEnumerable<BaseCSType> types, IEnumerable<string>? invalidNames = null,
        Func<string, NameContext, string>? fixPredicate = null, bool includeDefaultName = true)
    {
        HashSet<string> invalidNamesSet = invalidNames?.ToHashSet() ?? [];
        if (includeDefaultName)
        {
            invalidNamesSet.UnionWith(_csKeywords);
            invalidNamesSet.UnionWith(_contextualKeywords);
        }

        fixPredicate ??= (name, context) => context switch
        {
            NameContext.Struct => $"{name}Struct",
            NameContext.Method => $"{name}Method",
            NameContext.Field => $"{name}Value",
            NameContext.Parameter => $"{name}Value",
            NameContext.Local => name,
            _ => name,
        };

        List<CSStruct> structs = [];
        List<CSStaticClass> staticClasses = [];
        List<CSEnum> enums = [];
        foreach (var item in types)
        {
            if (item is CSStruct structItem)
            {
                structs.Add(structItem);
            }
            else if (item is CSStaticClass staticClassItem)
            {
                staticClasses.Add(staticClassItem);
            }
            else if (item is CSEnum enumItem)
            {
                enums.Add(enumItem);
            }
        }

        FixInvalidNames(structs, invalidNamesSet, fixPredicate, includeDefaultName);
        FixInvalidNames(staticClasses, invalidNamesSet, fixPredicate, includeDefaultName);
        FixInvalidNames(enums, invalidNamesSet, fixPredicate, includeDefaultName);
    }

    public static void FixInvalidNames(
        IEnumerable<CSStruct> structs, IEnumerable<string>? invalidNames = null,
        Func<string, NameContext, string>? fixPredicate = null, bool includeDefaultName = true)
    {
        HashSet<string> invalidNamesSet = invalidNames?.ToHashSet() ?? [];
        if (includeDefaultName)
        {
            invalidNamesSet.UnionWith(_csKeywords);
            invalidNamesSet.UnionWith(_contextualKeywords);
        }

        fixPredicate ??= (name, context) => context switch
        {
            NameContext.Struct => $"{name}Struct",
            NameContext.Method => $"{name}Method",
            NameContext.Field => $"{name}Value",
            NameContext.Parameter => $"{name}Value",
            NameContext.Local => name,
            _ => name,
        };

        bool IsInvalidName(string? name) => name != null && invalidNamesSet.Contains(name);

        foreach (var item in structs)
        {
            foreach (var nested in item.NestedTypes)
            {
                switch (nested)
                {
                    case CSStruct structItem:
                        FixInvalidNames([structItem], invalidNames, fixPredicate, includeDefaultName);
                        break;
                    case CSStaticClass staticClass:
                        FixInvalidNames([staticClass], invalidNames, fixPredicate, includeDefaultName);
                        break;
                }
            }

            if (IsInvalidName(item.Name))
            {
                item.Name = fixPredicate(item.Name!, NameContext.Struct);
            }
            foreach (var method in item.Methods)
            {
                if (IsInvalidName(method.Name))
                {
                    method.Name = fixPredicate(method.Name!, NameContext.Method);
                }
                for (int i = 0; i < method.Parameters.Count; i++)
                {
                    var parameter = method.Parameters[i];
                    if (IsInvalidName(parameter.Name))
                    {
                        method.Parameters.TryReplaceAt(i, new CSParameter(parameter.PrimarySource!, parameter.Type, fixPredicate(parameter.Name!, NameContext.Parameter), parameter.DefaultValue));
                    }
                }
            }

            foreach (var field in item.Fields)
            {
                if (IsInvalidName(field.Name))
                {
                    field.Name = fixPredicate(field.Name!, NameContext.Field);
                }
            }

        }
    }

    public static void FixInvalidNames(
        IEnumerable<CSStaticClass> staticClasses, IEnumerable<string>? invalidNames = null,
        Func<string, NameContext, string>? fixPredicate = null, bool includeDefaultName = true)
    {
        HashSet<string> invalidNamesSet = invalidNames?.ToHashSet() ?? [];
        if (includeDefaultName)
        {
            invalidNamesSet.UnionWith(_csKeywords);
            invalidNamesSet.UnionWith(_contextualKeywords);
        }

        fixPredicate ??= (name, context) => context switch
        {
            NameContext.Struct => $"{name}Struct",
            NameContext.Method => $"{name}Method",
            NameContext.Field => $"{name}Value",
            NameContext.Parameter => $"{name}Value",
            NameContext.Local => name,
            _ => name,
        };

        bool IsInvalidName(string? name) => name != null && invalidNamesSet.Contains(name);

        foreach (var item in staticClasses)
        {
            if (IsInvalidName(item.Name))
            {
                item.Name = fixPredicate(item.Name!, NameContext.Struct);
            }
            foreach (var method in item.Methods)
            {
                if (IsInvalidName(method.Name))
                {
                    method.Name = fixPredicate(method.Name!, NameContext.Method);
                }
                for (int i = 0; i < method.Parameters.Count; i++)
                {
                    var parameter = method.Parameters[i];
                    if (IsInvalidName(parameter.Name))
                    {
                        method.Parameters.TryReplaceAt(i, new CSParameter(parameter.PrimarySource!, parameter.Type, fixPredicate(parameter.Name!, NameContext.Parameter), parameter.DefaultValue));
                    }
                }
            }

            foreach (var field in item.Fields)
            {
                if (IsInvalidName(field.Name))
                {
                    field.Name = fixPredicate(field.Name!, NameContext.Field);
                }
            }

        }
    }

    public static void FixInvalidNames(
        IEnumerable<CSEnum> enums, IEnumerable<string>? invalidNames = null,
        Func<string, NameContext, string>? fixPredicate = null, bool includeDefaultName = true)
    {
        HashSet<string> invalidNamesSet = invalidNames?.ToHashSet() ?? [];
        if (includeDefaultName)
        {
            invalidNamesSet.UnionWith(_csKeywords);
            invalidNamesSet.UnionWith(_contextualKeywords);
        }

        fixPredicate ??= (name, context) => context switch
        {
            NameContext.Struct => $"{name}Struct",
            NameContext.Method => $"{name}Method",
            NameContext.Field => $"{name}Value",
            NameContext.Parameter => $"{name}Value",
            NameContext.Local => name,
            _ => name,
        };

        bool IsInvalidName(string? name) => name != null && invalidNamesSet.Contains(name);

        foreach (var item in enums)
        {
            if (IsInvalidName(item.Name))
            {
                item.Name = fixPredicate(item.Name!, NameContext.Struct);
            }
            foreach (var value in item.Values)
            {
                if (IsInvalidName(value.Name))
                {
                    value.Name = fixPredicate(value.Name!, NameContext.Field);
                }
            }
        }
    }
}
