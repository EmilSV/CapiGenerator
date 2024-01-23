using CppAst;
using CapiGenerator.CModel.Type;
using CapiGenerator.CModel;

namespace CapiGenerator.Parser;

public class TypedefParser : BaseParser
{
    public override void FirstPass(
        Guid compilationUnitId,
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel)
    {
        foreach (var compilation in compilations)
        {
            foreach (var cppTypedef in compilation.Typedefs)
            {
                FindTypeDefs(cppTypedef, typedef =>
                {
                    var type = TypeConverter.PartialConvert(compilationUnitId, typedef.ElementType);
                    var typedefName = typedef.Name;
                    outputChannel.OnReceiveTypedef(new CTypedef(compilationUnitId, typedefName, type));
                });
            }
        }
    }

    public void FindTypeDefs(CppTypedef typedef, Action<CppTypedef> onTypedefFound)
    {
        onTypedefFound(typedef);
        var innerType = typedef.ElementType;
        if (innerType is CppTypedef innerTypedef)
        {
            FindTypeDefs(innerTypedef, onTypedefFound);
        }
    }

    public override void SecondPass(
        CCompilationUnit compilationUnit,
        BaseParserInputChannel inputChannel
    )
    {
        foreach (var typedef in inputChannel.GetTypedefs())
        {
            typedef.OnSecondPass(compilationUnit);
        }
    }
}