using System.Diagnostics;
using CapiGenerator;
using CppAst;

static class StructGen
{
    public static void GenerateStructs(CppCompilation compilation, string outputPath)
    {
        Debug.WriteLine("Generating Structs...");

        var structs = compilation.Classes.Where(c => c.ClassKind == CppClassKind.Struct && c.IsDefinition == true);

        foreach (var structure in structs)
        {
            if (StructInfoCache.Get(structure) is { isUnsafe: true })
            {
                GenerateUnsafeStruct(structure, outputPath);
            }
            else
            {
                GenerateNormalStruct(structure, outputPath);
            }
        }
    }

    static void GenerateNormalStruct(CppClass structure, string outputPath)
    {
        (string Type, string Name)[] members = structure.Fields.Select(
            f => (TypeConverter.ConvertToCSharpType(f.Type), GetCsFieldName(f.Name))
        ).ToArray();

        bool hasAnyPointers = members.Any(m => m.Type.Contains("*"));
        string structName = TypeConverter.GetStructName(structure);

        using StreamWriter file = File.CreateText(Path.Combine(outputPath, $"{structName}.cs"));
        file.WriteLine("using System;");
        file.WriteLine("using System.Runtime.InteropServices;");
        file.WriteLine();
        file.WriteLine("namespace WebGpuSharp;");
        file.WriteLine();

        file.WriteLine("[StructLayout(LayoutKind.Sequential)]");
        file.WriteLine($"public{(hasAnyPointers ? " unsafe" : "")} partial struct {structName}");
        file.WriteLine("{");
        foreach (var member in members)
        {
            file.WriteLine($"\tpublic {member.Type} {member.Name};");
        }

        file.WriteLine();

        file.WriteLine($"\tpublic {structName}()");
        file.WriteLine("\t{");
        foreach (var member in members)
        {
            file.WriteLine($"\t\tthis.{member.Name} = default;");
        }
        file.WriteLine("\t}");

        file.WriteLine();

        bool hasChainedStructPointer = members.Any(m => m.Type.Contains("ChainedStruct*"));

        if (hasChainedStructPointer)
        {
            var memberNoChainedStructPointer = members.Where(m => !m.Type.Contains("ChainedStruct*")).ToArray();
            if (memberNoChainedStructPointer.Length > 0)
            {

                file.Write($"\tpublic {structName}(");

                for (int i = 0; i < memberNoChainedStructPointer.Length; i++)
                {
                    var member = memberNoChainedStructPointer[i];
                    file.Write($"{member.Type} {GetCsArgName(member.Name)} = default");
                    if (i < memberNoChainedStructPointer.Length - 1)
                    {
                        file.Write(", ");
                    }
                }
                file.WriteLine(")");
                file.WriteLine("\t{");
                foreach (var member in memberNoChainedStructPointer)
                {
                    file.WriteLine($"\t\tthis.{member.Name} = {GetCsArgName(member.Name)};");
                }
                file.WriteLine("\t}");
                file.WriteLine();
            }
        }

        file.Write($"\tpublic {structName}(");

        for (int i = 0; i < members.Length; i++)
        {
            var member = members[i];
            file.Write($"{member.Type} {GetCsArgName(member.Name)} = default");
            if (i < members.Length - 1)
            {
                file.Write(", ");
            }
        }
        file.WriteLine(")");
        file.WriteLine("\t{");
        foreach (var member in members)
        {
            file.WriteLine($"\t\tthis.{member.Name} = {GetCsArgName(member.Name)};");
        }
        file.WriteLine("\t}");

        file.WriteLine("}");
        file.WriteLine();
    }

    static void GenerateUnsafeStruct(CppClass structure, string outputPath)
    {
        (string Type, string Name)[] members = structure.Fields.Select(
            f => (TypeConverter.ConvertToCSharpType(f.Type), GetCsFieldName(f.Name))
        ).ToArray();

        bool hasAnyPointers = members.Any(m => m.Type.Contains("*"));
        string fullStructName = TypeConverter.GetStructName(structure);
        string structName = fullStructName.Split('.')[0];
        string innerStructName = fullStructName.Split('.')[1];

        using StreamWriter file = File.CreateText(Path.Combine(outputPath, $"{structName}.cs"));
        file.WriteLine("using System;");
        file.WriteLine("using System.Runtime.InteropServices;");
        file.WriteLine();
        file.WriteLine("namespace WebGpuSharp;");
        file.WriteLine();

        file.WriteLine($"public partial struct {structName}");
        file.WriteLine("{");


        file.WriteLine("\t[StructLayout(LayoutKind.Sequential)]");
        file.WriteLine($"\tpublic{(hasAnyPointers ? " unsafe" : "")} partial struct {innerStructName}");
        file.WriteLine("\t{");
        foreach (var member in members)
        {
            file.WriteLine($"\t\tpublic {member.Type} {member.Name};");
        }

        file.WriteLine();

        file.WriteLine($"\t\tpublic {innerStructName}()");
        file.WriteLine("\t\t{");
        foreach (var member in members)
        {
            file.WriteLine($"\t\t\tthis.{member.Name} = default;");
        }
        file.WriteLine("\t\t}");

        file.WriteLine();

        bool hasChainedStructPointer = members.Any(m => m.Type.Contains("ChainedStruct*"));

        if (hasChainedStructPointer)
        {
            var memberNoChainedStructPointer = members.Where(m => !m.Type.Contains("ChainedStruct*")).ToArray();
            if (memberNoChainedStructPointer.Length > 0)
            {
                file.Write($"\t\tpublic {innerStructName}(");

                for (int i = 0; i < memberNoChainedStructPointer.Length; i++)
                {
                    var member = memberNoChainedStructPointer[i];
                    file.Write($"{member.Type} {GetCsArgName(member.Name)} = default");
                    if (i < memberNoChainedStructPointer.Length - 1)
                    {
                        file.Write(", ");
                    }
                }
                file.WriteLine(")");
                file.WriteLine("\t\t{");
                foreach (var member in memberNoChainedStructPointer)
                {
                    file.WriteLine($"\t\t\tthis.{member.Name} = {GetCsArgName(member.Name)};");
                }
                file.WriteLine("\t\t}");
                file.WriteLine();
            }
        }

        file.Write($"\t\tpublic {innerStructName}(");

        for (int i = 0; i < members.Length; i++)
        {
            var member = members[i];
            file.Write($"{member.Type} {GetCsArgName(member.Name)} = default");
            if (i < members.Length - 1)
            {
                file.Write(", ");
            }
        }
        file.WriteLine(")");
        file.WriteLine("\t\t{");
        foreach (var member in members)
        {
            file.WriteLine($"\t\t\tthis.{member.Name} = {GetCsArgName(member.Name)};");
        }
        file.WriteLine("\t\t}");

        file.WriteLine("\t}");
        file.WriteLine("}");
        file.WriteLine();
    }

    static string GetCsArgName(string name)
    {
        if (char.IsUpper(name[0]))
        {
            return char.ToLowerInvariant(name[0]) + name[1..];
        }

        return name;
    }

    static string GetCsFieldName(string name)
    {
        if (char.IsLower(name[0]))
        {
            return char.ToUpperInvariant(name[0]) + name[1..];
        }

        return name;
    }
}