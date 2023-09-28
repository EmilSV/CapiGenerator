using System.Diagnostics;
using CapiGenerator;
using CppAst;

static class HandelGen
{
    public static void GeneratedHandles(CppCompilation compilation, string outputPath)
    {
        Debug.WriteLine("Generating Handles...");

        const string ptrType = "UIntPtr";
        const string nullValue = "UIntPtr.Zero";

        foreach (CppTypedef typedef in compilation.Typedefs)
        {
            if (!TypeConverter.IsHandelTypeDef(typedef))
            {
                continue;
            }
            string typeName = TypeConverter.GetHandelName(typedef);

            using StreamWriter file = File.CreateText(Path.Combine(outputPath, $"{typeName}.cs"));

            file.WriteLine("using System;");
            file.WriteLine("using System.Diagnostics;");
            file.WriteLine("using System.Runtime.InteropServices;\n");
            file.WriteLine("namespace WebGpuSharp;");
            file.WriteLine();
            file.WriteLine($"public readonly partial struct {typeName} : IEquatable<{typeName}>");
            file.WriteLine("{");
            file.WriteLine($"\tprivate readonly {ptrType} _ptr;");
            file.WriteLine($"\tpublic {typeName}({ptrType} ptr) {{ _ptr = ptr; }}");
            file.WriteLine($"\tpublic static {typeName} Null => new {typeName}({nullValue});");
            file.WriteLine($"\tpublic static explicit operator {typeName}({ptrType} ptr) => new {typeName}(ptr);");
            file.WriteLine($"\tpublic static explicit operator {ptrType}({typeName} handle) => handle._ptr;");
            file.WriteLine($"\tpublic static bool operator ==({typeName} left, {typeName} right) => left._ptr == right._ptr;");
            file.WriteLine($"\tpublic static bool operator !=({typeName} left, {typeName} right) => left._ptr != right._ptr;");
            file.WriteLine($"\tpublic static bool operator ==({typeName} left, {typeName}? right) => left._ptr == right.GetValueOrDefault()._ptr;");
            file.WriteLine($"\tpublic static bool operator !=({typeName} left, {typeName}? right) => left._ptr != right.GetValueOrDefault()._ptr;");
            file.WriteLine($"\tpublic static bool operator ==({typeName} left, {ptrType} right) => left._ptr == right;");
            file.WriteLine($"\tpublic static bool operator !=({typeName} left, {ptrType} right) => left._ptr != right;");
            file.WriteLine($"\tpublic {ptrType} GetAddress() => _ptr;");
            file.WriteLine($"\tpublic bool Equals({typeName} h) => _ptr == h._ptr;");
            file.WriteLine($"\tpublic override bool Equals(object? o) => (o is {typeName} h && Equals(h)) || (o is null && _ptr == UIntPtr.Zero);");
            file.WriteLine($"\tpublic override int GetHashCode() => _ptr.GetHashCode();");
            file.WriteLine("}");
            file.WriteLine();
        }

    }
}