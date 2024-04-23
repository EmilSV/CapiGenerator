using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;


public class CSConstTranslator(string className) : BaseTranslator
{
    protected virtual string NameSelector(CConstant value) => value.Name;
    protected virtual bool PredicateSelector(CConstant value) => true;

    public override void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        List<CSField> constantFields = [];
        List<CConstant> constantsTransLated = [];

        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var constant in compilationUnit.GetConstantEnumerable())
            {
                if (!PredicateSelector(constant))
                {
                    break;
                }

                constantFields.Add(TranslateConstant(constant));
                constantsTransLated.Add(constant);
            }
        }

        if(constantFields.Count == 0)
        {
            return;
        }

        var csStaticClass = new CSStaticClass
        {
            Name = className
        };
        csStaticClass.Fields.AddRange(constantFields);

        foreach (var constant in constantsTransLated)
        {
            constant.EnrichingDataStore.Add(new CSTranslationParentClassData(csStaticClass));
        }

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

    private CSField TranslateConstant(CConstant constant)
    {
        var cType = constant.Expression.GetTypeOfExpression();
        ICSType csType = cType switch
        {
            CConstantType.Char => CSPrimitiveType.Get(CSPrimitiveType.Kind.Byte),
            CConstantType.Int => CSPrimitiveType.Get(CSPrimitiveType.Kind.Long),
            CConstantType.Float => CSPrimitiveType.Get(CSPrimitiveType.Kind.Double),
            CConstantType.String => CSUft8LiteralType.Instance,
            _ => throw new Exception("Unknown constant type"),
        };

        bool IsStaticGetter = csType == CSUft8LiteralType.Instance;
        bool IsConstant = csType != CSUft8LiteralType.Instance;

        var typeInstance = new CSTypeInstance(csType);
        var csConstantExpression = CSConstantExpression.FromCConstantExpression(constant.Expression);
        var defaultValue = new CSDefaultValue(csConstantExpression);
        CSField newCSField;

        if (IsConstant)
        {
            newCSField = new CSField
            {
                Name = NameSelector(constant),
                Type = typeInstance,
                DefaultValue = defaultValue,
                IsConst = true
            };
        }
        else if (IsStaticGetter)
        {
            newCSField = new CSField
            {
                Name = NameSelector(constant),
                Type = typeInstance,
                IsStatic = true,
                GetterBody = new($" => {csConstantExpression}u8;"),
            };
        }
        else
        {
            throw new Exception("Unknown constant type");
        }

        newCSField.EnrichingDataStore.Add(new CSTranslationFromCAstData(constant));
        return newCSField;
    }


}