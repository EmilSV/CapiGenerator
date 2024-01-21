using System.Net.Http.Headers;
using CapiGenerator.CModel;
using CapiGenerator.Type;
using CppAst;

namespace CapiGenerator.Parser;


public class StructParser : BaseParser
{
    public override void FirstPass(
        Guid compilationUnitId,
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

                var newStruct = FirstPass(compilationUnitId, cppStruct);
                if (newStruct is not null)
                {
                    outputChannel.OnReceiveStruct(newStruct);
                }
            }
        }
    }

    protected virtual CStruct? FirstPass(Guid compilationUnitId, CppClass cppStruct)
    {
        var fields = cppStruct.Fields.Select(i => CppFieldToCField(compilationUnitId, i)).ToArray();
        if (fields == null || fields.Any(field => field is null))
        {
            OnError(cppStruct, "Failed to parse fields");
            return null;
        }

        return new CStruct(compilationUnitId, cppStruct.Name, fields!);
    }

    protected virtual bool ShouldSkip(CppClass cppStruct) => false;
    protected virtual void OnError(CppClass cppStruct, string message)
    {
        Console.Error.WriteLine($"Error parsing struct {cppStruct.Name}: {message}");
    }

    private static CField? CppFieldToCField(Guid compilationUnitId, CppField field)
    {
        var fieldType = TypeConverter.PartialConvert(compilationUnitId, field.Type);
        return new CField(compilationUnitId, field.Name, fieldType);
    }
}