using System.Diagnostics;
using System.Runtime.InteropServices;
using CapiGenerator.CModel;
using CapiGenerator.CModel.BuiltinConstants;

using CapiGenerator.CModel.ConstantToken;
using CapiGenerator.CModel.Type;
using CppAst;

namespace CapiGenerator.Parser;


public class ConstantParser : BaseParser
{
    private IReadOnlyDictionary<string, CppMacro> _macroFunctions = new Dictionary<string, CppMacro>();
    private IReadOnlySet<string> _typedefNames = new HashSet<string>();

    public override void FirstPass(
        ReadOnlySpan<CppCompilation> compilations,
        BaseParserOutputChannel outputChannel)
    {
        foreach (var compilation in compilations)
        {
            _macroFunctions = GetMacroFunctions(compilation);
            _typedefNames = GetTypedefNames(compilation);

            foreach (var macro in compilation.Macros)
            {
                if (macro.Parameters is not null)
                {
                    continue;
                }

                if (macro.Tokens.Count == 0)
                {
                    continue;
                }

                if (ShouldSkip(macro))
                {
                    continue;
                }

                var builtinConstant = AllBuiltinCConstants.AllCConstantTypes.FirstOrDefault(bc => bc.MacroIsBuiltin(macro));
                if (builtinConstant is not null)
                {
                    outputChannel.OnReceiveConstant(builtinConstant);
                    continue;
                }

                if (!LooksLikeConstantExpression(macro, _macroFunctions))
                {
                    continue;
                }

                var newConst = FirstPass(macro);
                if (newConst is not null)
                {
                    outputChannel.OnReceiveConstant(newConst);
                }
            }

            foreach (var field in compilation.Fields)
            {
                if (field.StorageQualifier != CppStorageQualifier.Static)
                {
                    continue;
                }

                if (field.Type is not CppQualifiedType type || type.Qualifier != CppTypeQualifier.Const)
                {
                    continue;
                }

                if (ShouldSkip(field))
                {
                    continue;
                }

                var newConstant = FirstPass(field);
                if (newConstant is not null)
                {
                    outputChannel.OnReceiveConstant(newConstant);
                }
            }
        }
    }

    public override void SecondPass(CCompilationUnit compilationUnit, BaseParserInputChannel inputChannel)
    {
        foreach (var constant in inputChannel.GetConstants())
        {
            SecondPass(constant, compilationUnit);
        }
    }

    protected virtual void SecondPass(BaseCConstant value, CCompilationUnit compilationUnit)
    {
        value.OnSecondPass(compilationUnit);
    }

    protected virtual CConstant? FirstPass(CppMacro macro)
    {
        switch (CConstant.From(macro, _macroFunctions, _typedefNames))
        {
            case { } constant:
                return constant;
            default:
                OnError(macro, "Failed to parse tokens");
                return null;
        }
    }

    protected virtual CStaticConstant? FirstPass(CppField field)
    {
        var constantType = CppTypeToConstantType(field.Type);
        if (constantType is null)
        {
            return null;
        }

        return new(
            realType: CTypeInstance.FromCppType(field.Type),
            constantType: constantType.Value,
            name: field.Name,
            expression: [new CConstLiteralToken(field.InitValue.Value!.ToString()!, field.Span.Start)]
        );
    }

    private static bool LooksLikeConstantExpression(
        CppMacro macro,
        IReadOnlyDictionary<string, CppMacro> macroFunctions)
    {
        var tokens = macro.Tokens
            .Where(token => token.Kind != CppTokenKind.Comment)
            .ToArray();
        if (!MacroFunctionExpander.TryExpand(tokens, macroFunctions, out var expandedTokens) ||
            expandedTokens.Length == 0)
        {
            return false;
        }

        foreach (var token in expandedTokens)
        {
            if (token.Kind is CppTokenKind.Identifier or CppTokenKind.Literal)
            {
                continue;
            }

            if (token.Kind != CppTokenKind.Punctuation ||
                !CConstantPunctuationToken.TryParse(token.Text, macro.Span.Start, out _))
            {
                return false;
            }
        }

        return true;
    }

    private static IReadOnlySet<string> GetTypedefNames(CppCompilation compilation)
    {
        HashSet<string> typedefNames = [];

        void AddTypedef(CppTypedef typedef)
        {
            typedefNames.Add(typedef.Name);
            if (typedef.ElementType is CppTypedef innerTypedef)
            {
                AddTypedef(innerTypedef);
            }
        }

        foreach (var typedef in compilation.Typedefs)
        {
            AddTypedef(typedef);
        }

        foreach (var macro in compilation.Macros)
        {
            if (macro.Parameters is null)
            {
                typedefNames.Remove(macro.Name);
            }
        }

        return typedefNames;
    }

    private static IReadOnlyDictionary<string, CppMacro> GetMacroFunctions(CppCompilation compilation)
    {
        Dictionary<string, CppMacro> macroFunctions = [];
        foreach (var macro in compilation.Macros)
        {
            if (macro.Parameters is not null)
            {
                macroFunctions[macro.Name] = macro;
            }
        }

        return macroFunctions;
    }

    protected virtual bool ShouldSkip(CppMacro constant) => false;
    protected virtual bool ShouldSkip(CppField constant) => false;
    protected virtual void OnError(CppMacro macro, string message)
    {
        var tokens = string.Join(", ", macro.Tokens.Select(token => $"{token.Kind}:'{token.Text}'"));
        Console.Error.WriteLine($"Error parsing constant {macro.Name}: {message}. Tokens: {tokens}");
    }

    protected virtual void OnError(CppField field, string message)
    {
        Console.Error.WriteLine($"Error parsing constant {field.Name}: {message}");
    }


    public static CConstantType? CppTypeToConstantType(CppType type)
    {
        static CppPrimitiveKind? GetPrimitiveType(CppType type)
        {
            if (type is CppPrimitiveType primitiveType)
            {
                return primitiveType.Kind;
            }
            if (type is CppTypedef typedef)
            {
                return GetPrimitiveType(typedef.ElementType);
            }
            if (type is CppQualifiedType qualifiedType && qualifiedType.Qualifier == CppTypeQualifier.Const)
            {
                return GetPrimitiveType(qualifiedType.ElementType);
            }
            return null;
        }

        var primitiveType = GetPrimitiveType(type);
        if (primitiveType is null)
        {
            return null;
        }

        return primitiveType switch
        {
            CppPrimitiveKind.Char => CConstantType.Char,
            CppPrimitiveKind.Int => CConstantType.Int,
            CppPrimitiveKind.UnsignedInt => CConstantType.UnsignedInt,
            CppPrimitiveKind.LongLong => CConstantType.LongLong,
            CppPrimitiveKind.UnsignedLongLong => CConstantType.UnsignedLongLong,
            CppPrimitiveKind.Float => CConstantType.Float,
            CppPrimitiveKind.Double => CConstantType.Double,
            CppPrimitiveKind.Short => CConstantType.Short,
            CppPrimitiveKind.Long => CConstantType.Long,
            CppPrimitiveKind.UnsignedChar => CConstantType.UnsignedChar,
            CppPrimitiveKind.UnsignedShort => CConstantType.UnsignedShort,
            CppPrimitiveKind.UnsignedLong => CConstantType.UnsignedLong,
            _ => null
        };
    }
}
