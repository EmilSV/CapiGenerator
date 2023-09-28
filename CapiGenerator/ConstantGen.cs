using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;
using CppAst;

static class ConstantGen
{
    private static readonly string[] constSkipList = {
         "_H_",
         "WGPU_EXPORT",
         "WGPU_SHARED_LIBRARY",
         "WGPU_IMPLEMENTATION",
         "_VGPU_EXTERN",
         "VGPU_API",
         "_WIN32",
         "WGPU_SKIP_PROCS"
    };

    static string NormalizeConstantValue(string value)
    {
        if (value.StartsWith("(") && value.EndsWith(")"))
            value = value[1..^1];

        return value switch
        {
            _ when value.EndsWith("UL", StringComparison.OrdinalIgnoreCase) => value.Replace("UL", "U"),
            _ when value.EndsWith("ULL", StringComparison.OrdinalIgnoreCase) => value.Replace("ULL", "UL"),
            _ => value
        };
    }

    static bool SkipConstant(CppMacro constant)
    {
        if (string.IsNullOrEmpty(constant.Value)
            || constSkipList.Contains(constant.Name))
            return true;

        return false;
    }

    static string GetName(string value)
    {
        const string PREFIX = "WGPU_";
        if (value.StartsWith(PREFIX))
        {
            return value[PREFIX.Length..];
        }

        return value;
    }


    static string GetConstantType(string value) => value switch
    {
        _ when value.EndsWith("UL", StringComparison.OrdinalIgnoreCase) => "ulong",
        _ when value.EndsWith("U", StringComparison.OrdinalIgnoreCase) => "uint",
        _ when value.EndsWith("F", StringComparison.OrdinalIgnoreCase) => "float",
        _ when uint.TryParse(value, out _) || value.StartsWith("0x") => "uint",
        _ => "string"
    };


    public static void GenerateConstants(CppCompilation compilation, string outputFolder)
    {
        Debug.WriteLine("Generating Constants...");
        using StreamWriter file = File.CreateText(Path.Combine(outputFolder, "WebGPU_Constants.cs"));
        file.WriteLine("namespace WebGpuSharp;");
        file.WriteLine();
        file.WriteLine("public static partial class WebGPU");
        file.WriteLine("{");
        foreach (var constant in compilation.Macros)
        {
            string constantValue = NormalizeConstantValue(constant.Value);

            if (SkipConstant(constant))
            {
                continue;
            }

            if (constant.Name == "WGPU_WHOLE_MAP_SIZE")
            {
                file.WriteLine($"\tpublic static readonly nuint WHOLE_MAP_SIZE = nuint.MaxValue;");
            }
            else
            {
                file.WriteLine($"\tpublic const {GetConstantType(constantValue)} {GetName(constant.Name)} = {constantValue};");
            }
        }
        file.WriteLine("}");
    }
}