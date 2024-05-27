using CppAst;
using CapiGenerator.CModel.Type;
using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinTypedefs;

namespace CapiGenerator.Parser;

public class TypedefParser : BaseParser
{
    public override void FirstPass(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel)
    {
        foreach (var compilation in compilations)
        {
            foreach (var cppTypedef in compilation.Typedefs)
            {
                var builtinTypedefs = AllBuiltinTypedefs.AllTypedefs.FirstOrDefault(typedef => typedef.TypedefIsBuiltin(cppTypedef));
                if (builtinTypedefs != null)
                {
                    outputChannel.OnReceiveBuiltinTypedef(builtinTypedefs);
                    continue;
                }

                FindTypeDefs(cppTypedef, typedef =>
                {
                    var type = CTypeInstance.FromCppType(typedef.ElementType);
                    var typedefName = typedef.Name;
                    outputChannel.OnReceiveTypedef(new CTypedef(typedefName, type));
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