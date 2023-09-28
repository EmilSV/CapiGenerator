using System.Diagnostics;
using System.Text;
using CapiGenerator;
using CppAst;

static class FunctionGen
{
    public static void GenerateFunctions(CppCompilation compilation, string outputFolder)
    {
        Debug.WriteLine("Generating function...");

        using StreamWriter file = File.CreateText(Path.Combine(outputFolder, "WebGPU.cs"));
        file.WriteLine("using System;");
        file.WriteLine("using System.Diagnostics;");
        file.WriteLine("using System.Runtime.InteropServices;\n");
        file.WriteLine("namespace WebGpuSharp;");
        file.WriteLine();
        file.WriteLine("public static unsafe partial class WebGPU");
        file.WriteLine("{");

        foreach (var function in compilation.Functions)
        {
            if (ShouldSkip(function))
            {
                continue;
            }

            string convertedType = TypeConverter.ConvertToCSharpType(function.ReturnType, false);

            file.WriteLine($"\n\t[DllImport(\"webgpu_dawn\", CallingConvention = CallingConvention.Cdecl, EntryPoint = \"{function.Name}\")]");
            file.WriteLine($"\tpublic static extern {convertedType} {GetFunctionName(function)}({GetParametersSignature(function)});");
        }

        file.WriteLine("}");
    }

    static bool ShouldSkip(CppFunction function) => function.Name switch
    {
        "wgpuGetProcAddress" => true,
        _ => false
    };

    static string GetFunctionName(CppFunction function)
    {
        const string PREFIX = "wgpu";
        string name = function.Name;
        if (name.StartsWith(PREFIX))
        {
            name = name[PREFIX.Length..];
        }
        return name;
    }

    static string GetParametersSignature(CppFunction command)
    {
        StringBuilder signature = new();
        foreach (var parameter in command.Parameters)
        {
            string convertedType = TypeConverter.ConvertToCSharpType(parameter.Type);
            signature.Append($"{convertedType} {parameter.Name}, ");
        }

        if (signature.Length > 2)
        {
            signature.Length -= 2;
        }
        return signature.ToString();
    }

}