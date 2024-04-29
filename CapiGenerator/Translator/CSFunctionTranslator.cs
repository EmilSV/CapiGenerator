using System.Collections.Immutable;
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
        if (methods.Count == 0)
        {
            return;
        }

        var csStaticClass = new CSStaticClass
        {
            Name = className,
        };
        csStaticClass.Methods.AddRange(methods);

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

        CSMethod method = new()
        {
            ReturnType = CSTypeInstance.CreateFromCTypeInstance(function.ReturnType),
            Name = NameSelector(function),
            IsExtern = true,
            IsStatic = true
        };
        method.Parameters.AddRange(function.Parameters.ToArray().Select(CSParameter.FromCParameter));

        method.EnrichingDataStore.Add(new CSTranslationFromCAstData(function));
        if (dllName is not null)
        {
            method.Attributes.Add(CSAttribute<DllImportAttribute>.Create(

                [$"\"{dllName}\""],
                [
                    new("CallingConvention", "System.Runtime.InteropServices.CallingConvention.Cdecl"),
                    new("EntryPoint", $"\"{function.Name}\"")
                ]
            ));
        }
        function.EnrichingDataStore.Add(new CTranslationToCSAstData(method));

        return method;
    }
}
