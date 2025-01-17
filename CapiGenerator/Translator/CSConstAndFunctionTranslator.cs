using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.BuiltinConstants;
using CapiGenerator.CSModel.ConstantToken;
using CapiGenerator.CSModel.EnrichData;
using CapiGenerator.Parser;

namespace CapiGenerator.Translator;

public class CSConstAndFunctionTranslator(string className, string dllName) : BaseTranslator
{
    protected virtual string NameSelector(BaseCConstant value) => value.Name;
    protected virtual bool PredicateSelector(BaseCConstant value) => true;

    protected virtual string NameSelector(CFunction value) => value.Name;
    protected virtual bool PredicateSelector(CFunction value) => true;

    public override void FirstPass(
        CSTranslationUnit translationUnit,
        ReadOnlySpan<CCompilationUnit> compilationUnits,
        BaseTranslatorOutputChannel outputChannel)
    {
        var (constantFields, constantsTransLated) = TranslateConstantsFirstPass(
            translationUnit,
            compilationUnits,
            outputChannel
        );

        var methods = TranslateFunctionsFirstPass(
            translationUnit,
            compilationUnits,
            outputChannel
        );

        if (constantFields.Count == 0 && methods.Count == 0)
        {
            return;
        }

        var csStaticClass = new CSStaticClass
        {
            Name = className
        };
        csStaticClass.Fields.AddRange(constantFields);
        csStaticClass.Methods.AddRange(methods);

        foreach (var constant in constantsTransLated)
        {
            constant.EnrichingDataStore.Set(new CSTranslationParentClassData(csStaticClass));
        }

        outputChannel.OnReceiveStaticClass(csStaticClass);
    }

    public override void SecondPass(
        CSTranslationUnit translationUnit,
        BaseTranslatorInputChannel inputChannel)
    {
        ConstantsSecondPass(translationUnit, inputChannel);
        FunctionSecondPass(translationUnit, inputChannel);
    }

    protected static void FunctionSecondPass(
        CSTranslationUnit translationUnit,
        BaseTranslatorInputChannel inputChannel
    )
    {
        foreach (var staticClass in inputChannel.GetStaticClasses())
        {
            staticClass.OnSecondPass(translationUnit);
        }
    }

    protected static void ConstantsSecondPass(
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


    private (List<CSField> constantFields, List<BaseCConstant> constantsTransLated) TranslateConstantsFirstPass(
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
                    case CStaticConstant staticConstant:
                        constantFields.Add(TranslateConstant(staticConstant));
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

        return (constantFields, constantsTransLated);
    }

    private List<CSMethod> TranslateFunctionsFirstPass(
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

        return methods;
    }

    protected CSField TranslateConstant(CConstant constant)
    {
        var cType = constant.GetCConstantType();
        ICSType csType = cType switch
        {
            CConstantType.Char or CConstantType.Int8_t or CConstantType.UInt8_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.Byte),
            CConstantType.Int or CConstantType.Int32_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.Int),
            CConstantType.UnsignedInt or CConstantType.UInt32_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.UInt),
            CConstantType.LongLong or CConstantType.Int64_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.Long),
            CConstantType.UnsignedLongLong or CConstantType.UInt64_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.ULong),
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

        newCSField.EnrichingDataStore.Set(new CSTranslationFromCAstData(constant));
        return newCSField;
    }

    protected CSField TranslateConstant(CStaticConstant constant)
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
            CConstantType.Short => CSPrimitiveType.Get(CSPrimitiveType.Kind.Short),
            CConstantType.Long => CSPrimitiveType.Get(CSPrimitiveType.Kind.Long),
            CConstantType.UnsignedShort => CSPrimitiveType.Get(CSPrimitiveType.Kind.UShort),
            CConstantType.UnsignedLong => CSPrimitiveType.Get(CSPrimitiveType.Kind.ULong),
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

        newCSField.EnrichingDataStore.Set(new CSTranslationFromCAstData(constant));
        return newCSField;
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

        method.EnrichingDataStore.Set(new CSTranslationFromCAstData(function));
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
        function.EnrichingDataStore.Set(new CTranslationToCSAstData(method));

        return method;
    }
}