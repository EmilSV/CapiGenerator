using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSFunctionTranslator(string className, string dllName) : BaseTranslator
{
    protected virtual string NameSelector(CFunction value) => value.Name;
    protected virtual bool PredicateSelector(CFunction value) => true;

    public override void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        List<CSMethod> methods = [];

        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var function in compilationUnit.GetFunctionEnumerable())
            {
                if (!PredicateSelector(function)) 
                {
                    break;
                }

                methods.Add(TranslateFunction(function));
            }
        }

        var csStaticClass = new CSStaticClass(className, [], CollectionsMarshal.AsSpan(methods));

        outputChannel.OnReceiveStaticClass(csStaticClass);
    }

    public override void SecondPass(
        CSTranslationUnit translationUnit,
        BaseTranslatorInputChannel inputChannel)
    {
        foreach (var staticClass in inputChannel.GetStaticClasses())
        {
            staticClass.OnSecondPass(translationUnit);
        }
    }


    protected CSMethod TranslateFunction(CFunction function)
    {
        CTypeInstance returnType = function.ReturnType;

        CSMethod method = new(
            returnType: CSTypeInstance.CreateFromCTypeInstance(function.ReturnType),
            name: NameSelector(function),
            parameters: function.Parameters.ToArray().Select(CSParameter.FromCParameter).ToArray()
        )
        {
            IsExtern = true,
            IsStatic = true,
        };

        method.EnrichingDataStore.Add(new CSTranslationFromCAstData(function));
        method.EnrichingDataStore.Add(new CSAttributesData([
            new DllImportAttribute(dllName)
            {
                CallingConvention = CallingConvention.Cdecl,
                EntryPoint = function.Name,   
            }
        ]));
        function.EnrichingDataStore.Add(new CTranslationToCSAstData(method));

        return method;
    }
}
