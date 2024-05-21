using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.BuiltinConstants;
using CapiGenerator.CSModel.ConstantToken;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;


public class CSConstTranslator(string className) : BaseTranslator
{
    protected virtual string NameSelector(BaseCConstant value) => value.Name;
    protected virtual bool PredicateSelector(BaseCConstant value) => true;

    public override void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        List<CSField> constantFields = [];
        List<BaseCConstant> constantsTransLated = [];


        foreach (var compilationUnit in compilationUnits)
        {
            foreach (var constant in compilationUnit.GetConstantEnumerable())
            {
                if (!PredicateSelector(constant))
                {
                    break;
                }
                switch (constant)
                {
                    case CConstant cConstant:
                        constantFields.Add(TranslateConstant(cConstant));
                        constantsTransLated.Add(constant);
                        break;
                    case BaseBuiltInCConstant builtinConstant:
                        var builtinCsConstant = AllBuiltInCsConstants.csConstants.First(i => i.CConstantTranslateToBuiltin(builtinConstant));
                        outputChannel.OnReceiveBuiltInConstant(builtinConstant, builtinCsConstant);
                        break;
                    default: throw new Exception("Unknown constant type");
                }
            }
        }

        if (constantFields.Count == 0)
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
        static bool ExpressionFullyConstant(CSConstantExpression expression)
        {
            foreach (var token in expression.Tokens)
            {
                if (token is CSConstIdentifierToken identifierToken)
                {
                    var fieldLike = identifierToken.GetField();
                    if (fieldLike is BaseBuiltInCsConstant builtInConstant && !builtInConstant.HasConstantValue())
                    {
                        return false;
                    }
                    else if (fieldLike is CSField field &&
                    field.DefaultValue.TryGetCSConstantExpression(out var csConstantExpression) &&
                    !ExpressionFullyConstant(csConstantExpression))
                    {
                        return false;
                    }

                }
            }

            return true;
        }

        foreach (var staticClass in inputChannel.GetStaticClasses())
        {
            staticClass.OnSecondPass(translationUnit);

        }

        foreach (var staticClass in inputChannel.GetStaticClasses())
        {
            foreach (var field in staticClass.Fields)
            {
                if (field.DefaultValue.TryGetCSConstantExpression(out var csConstantExpression) &&
                    !ExpressionFullyConstant(csConstantExpression))
                {
                    field.IsConst = false;
                    field.IsReadOnly = true;
                    field.IsStatic = true;
                }
            }
        }
    }

    private CSField TranslateConstant(CConstant constant)
    {
        var cType = constant.GetCConstantType();
        ICSType csType = cType switch
        {
            CConstantType.Char => CSPrimitiveType.Get(CSPrimitiveType.Kind.Byte),
            CConstantType.Int => CSPrimitiveType.Get(CSPrimitiveType.Kind.Int),
            CConstantType.UnsignedInt => CSPrimitiveType.Get(CSPrimitiveType.Kind.UInt),
            CConstantType.LongLong => CSPrimitiveType.Get(CSPrimitiveType.Kind.Long),
            CConstantType.UnsignedLongLong => CSPrimitiveType.Get(CSPrimitiveType.Kind.ULong),
            CConstantType.Float => CSPrimitiveType.Get(CSPrimitiveType.Kind.Double),
            CConstantType.Size_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.NUInt),
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