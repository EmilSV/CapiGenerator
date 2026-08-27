using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;
using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CapiGenerator.CSModel;
using CapiGenerator.CSModel.BuiltinConstants;
using CapiGenerator.CSModel.ConstantToken;
using CapiGenerator.CSModel.Comments;
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
                        constantFields.Add(TranslateConstant(cConstant, compilationUnit));
                        constantsTransLated.Add(constant);
                        break;
                    case BaseBuiltInCConstant builtinConstant:
                        var builtinCsConstant = AllBuiltInCsConstants.csConstants.First(i => i.CConstantTranslateToBuiltin(builtinConstant));
                        outputChannel.OnReceiveBuiltInConstant(builtinConstant, builtinCsConstant);
                        break;
                    case CStaticConstant staticConstant:
                        constantFields.Add(TranslateConstant(staticConstant));
                        constantsTransLated.Add(constant);
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

    private CSField TranslateConstant(CConstant constant, CCompilationUnit compilationUnit)
    {
        var cType = constant.GetCConstantType();
        ICSType csType = cType switch
        {
            CConstantType.Char or CConstantType.UInt8_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.Byte),
            CConstantType.Int8_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.SByte),
            CConstantType.Short or CConstantType.Int16_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.Short),
            CConstantType.UnsignedShort or CConstantType.UInt16_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.UShort),
            CConstantType.Int or CConstantType.Int32_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.Int),
            CConstantType.UnsignedInt or CConstantType.UInt32_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.UInt),
            CConstantType.LongLong or CConstantType.Int64_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.Long),
            CConstantType.UnsignedLongLong or CConstantType.UInt64_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.ULong),
            CConstantType.IntPtr_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.NInt),
            CConstantType.UIntPtr_t or CConstantType.Size_t => CSPrimitiveType.Get(CSPrimitiveType.Kind.NUInt),
            CConstantType.Float => CSPrimitiveType.Get(CSPrimitiveType.Kind.Float),
            CConstantType.Double => CSPrimitiveType.Get(CSPrimitiveType.Kind.Double),
            CConstantType.String => CSUft8LiteralType.Instance,
            _ => throw new InvalidOperationException($"Unsupported constant type {cType} for {constant.Name}"),
        };

        bool IsStaticGetter = csType == CSUft8LiteralType.Instance;
        bool IsConstant = csType != CSUft8LiteralType.Instance;
        bool IsNativeInteger = cType is
            CConstantType.IntPtr_t or CConstantType.UIntPtr_t or CConstantType.Size_t;

        var typeInstance = new CSTypeInstance(csType);
        var csConstantExpression = CSConstantExpression.FromCConstantExpression(constant.Expression);
        var defaultValue = new CSDefaultValue(csConstantExpression);
        CSField newCSField;

        if (IsConstant)
        {
            newCSField = new CSField(constant)
            {
                Name = NameSelector(constant),
                Type = typeInstance,
                DefaultValue = defaultValue,
                IsConst = !IsNativeInteger,
                IsStatic = IsNativeInteger,
                IsReadOnly = IsNativeInteger
            };
        }
        else if (IsStaticGetter)
        {
            newCSField = new CSField(constant)
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

        newCSField.Comments = CCommentTranslator.Translate(constant.Comment, compilationUnit);
        constant.AddDerivative(newCSField);
        return newCSField;
    }

    private CSField TranslateConstant(CStaticConstant constant)
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
        bool IsNativeInteger = cType is
            CConstantType.IntPtr_t or CConstantType.UIntPtr_t or CConstantType.Size_t;

        var typeInstance = new CSTypeInstance(csType);
        var csConstantExpression = CSConstantExpression.FromCConstantExpression(constant.Expression);
        var defaultValue = new CSDefaultValue(csConstantExpression);
        CSField newCSField;

        if (IsConstant)
        {
            newCSField = new CSField(constant)
            {
                Name = NameSelector(constant),
                Type = typeInstance,
                DefaultValue = defaultValue,
                IsConst = !IsNativeInteger,
                IsStatic = IsNativeInteger,
                IsReadOnly = IsNativeInteger
            };
        }
        else if (IsStaticGetter)
        {
            newCSField = new CSField(constant)
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

        constant.AddDerivative(newCSField);
        return newCSField;
    }


}
