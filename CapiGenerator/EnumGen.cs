using System.Diagnostics;
using CppAst;

static class EnumGen
{
    public static void GenerateEnums(CppCompilation compilation, string outputPath)
    {
        // Debug.WriteLine("Generating Enums...");

        // foreach (var cppEnum in compilation.Enums)
        // {
        //     string enumName = TypeConverter.GetEnumName(cppEnum);
        //     using StreamWriter file = File.CreateText(Path.Combine(outputPath, $"{enumName}.cs"));
        //     file.WriteLine("namespace WebGpuSharp;");

        //     file.WriteLine();

        //     bool isFlag = false;
        //     if (compilation.Typedefs.Any(t => t.Name == cppEnum.Name + "Flags"))
        //     {
        //         file.WriteLine("[System.Flags]");
        //         isFlag = true;
        //     }


        //     file.WriteLine($"public enum {enumName} : {(isFlag ? "uint" : "int")}");
        //     file.WriteLine("{");

        //     foreach (var member in cppEnum.Items)
        //     {
        //         string cleanMemberName = member.Name.Split('_')[1];
        //         if (char.IsNumber(cleanMemberName, 0))
        //             cleanMemberName = $"_{cleanMemberName}";

        //         if (cleanMemberName == "Force32")
        //         {
        //             continue;
        //         }

        //         file.WriteLine($"\t{cleanMemberName} = {member.Value},");
        //     }

        //     file.WriteLine("}\n");
        // }
    }
}