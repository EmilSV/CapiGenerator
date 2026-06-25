using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CppAst;

namespace CapiGenerator.Parser;

public class UnionParser : BaseParser
{
    public override void FirstPass(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel)
    {
        foreach (var compilation in compilations)
        {
            foreach (var cppUnion in compilation.Classes)
            {
                if (cppUnion == null)
                {
                    continue;
                }

                if (ShouldSkip(cppUnion))
                {
                    continue;
                }

                var newUnion = FirstPass(cppUnion);
                if (newUnion is not null)
                {
                    outputChannel.OnReceiveUnion(newUnion);
                }
            }
        }
    }

    protected virtual CUnion? FirstPass(CppClass cppUnion)
    {
        var fields = cppUnion.Fields.Select(CppFieldToCField).ToArray();
        if (fields == null || fields.Any(field => field is null))
        {
            OnError(cppUnion, "Failed to parse fields");
            return null;
        }

        return new CUnion(
            cppUnion.Name,
            fields!,
            checked((int)cppUnion.SizeOf),
            checked((int)cppUnion.AlignOf));
    }

    public override void SecondPass(CCompilationUnit compilationUnit, BaseParserInputChannel inputChannel)
    {
        foreach (var cUnion in inputChannel.GetUnions())
        {
            cUnion.OnSecondPass(compilationUnit);
        }
    }

    protected virtual bool ShouldSkip(CppClass cppUnion) =>
        cppUnion.ClassKind != CppClassKind.Union;

    protected virtual void OnError(CppClass cppUnion, string message)
    {
        Console.Error.WriteLine($"Error parsing union {cppUnion.Name}: {message}");
    }

    private static CField? CppFieldToCField(CppField field)
    {
        var fieldType = CTypeInstance.FromCppType(field.Type);
        return new CField(field.Name, fieldType, checked((int)field.Offset));
    }
}
