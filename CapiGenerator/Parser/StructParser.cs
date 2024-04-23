using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CppAst;

namespace CapiGenerator.Parser;


public class StructParser : BaseParser
{
    public override void FirstPass(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel)
    {
        foreach (var compilation in compilations)
        {
            foreach (var cppStruct in compilation.Classes)
            {
                if (cppStruct == null)
                {
                    continue;
                }

                if (ShouldSkip(cppStruct))
                {
                    continue;
                }

                var newStruct = FirstPass(cppStruct);
                if (newStruct is not null)
                {
                    outputChannel.OnReceiveStruct(newStruct);
                }
            }
        }
    }

    protected virtual CStruct? FirstPass(CppClass cppStruct)
    {
        var fields = cppStruct.Fields.Select(i => CppFieldToCField(i)).ToArray();
        if (fields == null || fields.Any(field => field is null))
        {
            OnError(cppStruct, "Failed to parse fields");
            return null;
        }

        return new CStruct(cppStruct.Name, fields!);
    }

    public override void SecondPass(CCompilationUnit compilationUnit, BaseParserInputChannel inputChannel)
    {
        foreach (var cStruct in inputChannel.GetStructs())
        {
            cStruct.OnSecondPass(compilationUnit);
        }
    }

    protected virtual bool ShouldSkip(CppClass cppStruct) => false;
    protected virtual void OnError(CppClass cppStruct, string message)
    {
        Console.Error.WriteLine($"Error parsing struct {cppStruct.Name}: {message}");
    }

    private static CField? CppFieldToCField(CppField field)
    {
        var fieldType = CTypeInstance.FromCppType(field.Type);
        return new CField(field.Name, fieldType);
    }
}